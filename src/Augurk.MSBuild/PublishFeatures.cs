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
        /// Gets the uri that should be used to post the features to.
        /// </summary>
        private string TargetUri{get { return String.Format(CultureInfo.InvariantCulture, "{0}/api/features/{1}/{2}", AugurkUri.TrimEnd('/'), BranchName, GroupName ?? "Default"); }}

        /// <summary>
        /// Publishes the features provided through the <see cref="FeatureFiles"/> property 
        /// to the Augurk site hosted at the <see cref="TargetUri"/>.
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

            // Clear any existing features in this group, if required
            if (ClearGroupBeforePublish)
            {
                Log.LogMessage("Clearing existing features in group {0} for branch {1}.", GroupName ?? "Default", BranchName);
                client.DeleteAsync(TargetUri).Wait();
            }

            // Parse the restresults
            Dictionary<string, FeatureTestResult> testResults = ParseTestResults();

            // Parse and publish each of the provided feature files
            foreach (var featureFile in FeatureFiles)
            {
                try
                {
                    using (TextReader reader = File.OpenText(featureFile.ItemSpec))
                    {
                        // Parse the feature and convert it to the correct format
                        Feature feature = parser.Parse(reader, featureFile.ItemSpec).ConvertToFeature();

                        // Add the testresults, if present
                        if (testResults.ContainsKey(feature.Title))
                        {
                            FeatureTestResult featureTestResult = testResults[feature.Title];
                            feature.TestResult = featureTestResult.Result;

                            foreach (var scenario in feature.Scenarios)
                            {
                                scenario.TestResult =
                                    featureTestResult.ScenarioTestResults
                                                     .Where(scenarioTestResult => scenarioTestResult.ScenarioTitle == scenario.Title)
                                                     .Max(scenarioTestResult => scenarioTestResult.Result);
                            }
                        }

                        // Publish the feature
                        var postTask = client.PostAsJsonAsync<Feature>(TargetUri, feature);
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
                                         TargetUri,
                                         postTask.Result.StatusCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.LogErrorFromException(e);
                }
            }

            return result;
        }

        private Dictionary<string, FeatureTestResult> ParseTestResults()
        {
            if (TestResultsFile == null)
            {
                Log.LogMessage("No testresults file has been provided, continuing without test results...");
                return new Dictionary<string, FeatureTestResult>();
            }

            ITestResultParser parser = GetTestResultParser(TestEngine);

            Log.LogMessage("Testresult has been configured to be {0}, available results will be added to features...", TestEngine);
            return parser.Parse(TestResultsFile.ItemSpec).ToDictionary(result => result.FeatureTitle);
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
    }
}
