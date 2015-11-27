/*
 Copyright 2014, Mark Taling
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using Augurk.Entities;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Augurk.Entities.Test;
using Augurk.MSBuild.TestResultParsers;
using TechTalk.SpecFlow.Parser;

namespace Augurk.MSBuild
{
    /// <summary>
    /// Provides functionality to publish features and/or testresults to an Augurk endpoint.
    /// </summary>
    public class PublishFeatures : Task
    {
        /// <summary>
        /// Gets or sets the base uri of the Augurk website.
        /// </summary>
        [Required]
        public string AugurkUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the branch the features currently reside in.
        /// </summary>
        [Required]
        public string BranchName { get; set; }

        /// <summary>
        /// Gets or sets the features that should be published.
        /// </summary>
        [Required]
        public ITaskItem[] FeatureFiles { get; set; }

        /// <summary>
        /// Gets or sets the name under which these features should be grouped.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Boolean"/>value indicating whether the group should be cleared before publishing the features.
        /// </summary>
        public bool ClearGroupBeforePublish { get; set; }

        /// <summary>
        /// Gets or sets the language that should be used when parsing the feature files.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the file containing the testresults
        /// </summary>
        public ITaskItem TestResultsFile { get; set; }

        /// <summary>
        /// Gets or sets the testengine that generate the testresults.
        /// If not provided this value defaults to <c>TestEngine.MsTest</c>
        /// </summary>
        public TestEngine TestEngine { get; set; }

        /// <summary>
        /// Publishes the features provided through the <see cref="FeatureFiles"/> property 
        /// to the Augurk site hosted at the <see cref="AugurkUri"/>.
        /// </summary>
        /// <returns>
        /// true if the publish was completed successfully; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            bool result = true;

            // Instantiate a new parser, using the provided language
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo(Language ?? "en-US"));
            var client = new HttpClient();

            // Get the base uri for all further operations
            string groupUri = GetGroupUri();

            // Clear any existing features in this group, if required
            if (ClearGroupBeforePublish)
            {
                Log.LogMessage("Clearing existing features in group {0} for branch {1}.", GroupName ?? "Default", BranchName);
                client.DeleteAsync(groupUri).Wait();
            }

            // Parse and publish each of the provided feature files
            foreach (var featureFile in FeatureFiles)
            {
                try
                {
                    using (TextReader reader = File.OpenText(featureFile.ItemSpec))
                    {
                        // Parse the feature and convert it to the correct format
                        Feature feature = parser.Parse(reader, featureFile.ItemSpec).ConvertToFeature();

                        // Get the uri to which the feature should be published
                        string targetUri = GetFeatureUri(groupUri, feature.Title);

                        // Publish the feature
                        var postTask = client.PostAsJsonAsync<Feature>(targetUri, feature);
                        postTask.Wait();

                        // Process the result
                        if (postTask.Result.IsSuccessStatusCode)
                        {
                            Log.LogMessage("Succesfully published feature '{0}' to group {1} for branch {2}.",
                                           feature.Title, 
                                           GroupName ?? "Default", 
                                           BranchName);

                        }
                        else
                        {
                            result = false;
                            Log.LogError("Publishing feature '{0}' to uri '{1}' resulted in statuscode '{2}'",
                                         feature.Title,
                                         targetUri,
                                         postTask.Result.StatusCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.LogErrorFromException(e);
                }
            }

            // Parse the restresults
            IEnumerable<FeatureTestResult> testResults = ParseTestResults();

            // Publish the testresults, if present
            foreach (var featureTestResult in testResults)
            {
                try
                {
                    // Get the uri to which the test result should be published
                    string targetUri = GetTestResultUri(groupUri, featureTestResult.FeatureTitle);

                    // Publish the test result
                    var postTask = client.PostAsJsonAsync<FeatureTestResult>(targetUri, featureTestResult);
                    postTask.Wait();

                    // Process the result
                    if (postTask.Result.IsSuccessStatusCode)
                    {
                        Log.LogMessage("Succesfully published test result of feature '{0}' to group {1} for branch {2}.",
                                       featureTestResult.FeatureTitle,
                                       GroupName ?? "Default",
                                       BranchName);

                    }
                    else
                    {
                        result = false;
                        Log.LogError("Publishing test result of feature '{0}' to uri '{1}' resulted in statuscode '{2}'",
                                     featureTestResult.FeatureTitle,
                                     targetUri,
                                     postTask.Result.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    Log.LogErrorFromException(e);
                }
            }

            return result;
        }

        private IEnumerable<FeatureTestResult> ParseTestResults()
        {
            if (TestResultsFile == null)
            {
                Log.LogMessage("No testresults file has been provided, continuing without test results...");
                return Enumerable.Empty<FeatureTestResult>();
            }

            ITestResultParser parser = GetTestResultParser(TestEngine);

            Log.LogMessage("Testresult has been configured to be {0}, available results will be published.", TestEngine);
            return parser.Parse(TestResultsFile.ItemSpec);
        }

        private ITestResultParser GetTestResultParser(TestEngine testEngine)
        {
            switch (testEngine)
            {
                case TestEngine.MsTest:
                    return new MsTestResultParser();
                default:
                    throw new NotSupportedException();
            }
        }

        private string GetGroupUri()
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "{0}/api/features/{1}/{2}",
                                 AugurkUri.TrimEnd('/'),
                                 BranchName,
                                 GroupName ?? "Default");
        }

        private string GetTestResultUri(string groupUri, string featureTitle)
        {
            return String.Format(CultureInfo.InvariantCulture, 
                                 "{0}/{1}/testresult", 
                                 groupUri, 
                                 featureTitle);
        }

        private string GetFeatureUri(string groupUri, string featureTitle)
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "{0}/{1}/",
                                 groupUri,
                                 featureTitle);
        }
    }
}
