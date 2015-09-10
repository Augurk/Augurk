using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Augurk.CommandLine.Options;
using Augurk.Entities;
using TechTalk.SpecFlow.Parser;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the publish command.
    /// </summary>
    internal class PublishCommand
    {
        /// <summary>
        /// Stores the <see cref="PublishOptions"/> passed to the command.
        /// </summary>
        private PublishOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishCommand"/> class.
        /// </summary>
        /// <param name="options">An <see cref="PublishOptions"/> instance to use.</param>
        public PublishCommand(PublishOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            // Instantiate a new parser, using the provided language
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo(_options.Language ?? "en-US"));
            var client = new HttpClient();

            // Get the base uri for all further operations
            string groupUri = GetGroupUri();

            // Clear any existing features in this group, if required
            if (_options.ClearGroup)
            {
                Console.WriteLine("Clearing existing features in group {0} for branch {1}.", _options.GroupName ?? "Default", _options.BranchName);
                client.DeleteAsync(groupUri).Wait();
            }

            // Parse and publish each of the provided feature files
            foreach (var featureFile in _options.FeatureFiles)
            {
                try
                {
                    using (TextReader reader = File.OpenText(featureFile))
                    {
                        // Parse the feature and convert it to the correct format
                        Feature feature = parser.Parse(reader, featureFile).ConvertToFeature();
                        feature.Product = "SomeProduct";

                        // Get the uri to which the feature should be published
                        string targetUri = GetFeatureUri(groupUri, feature.Title);

                        // Publish the feature
                        var postTask = client.PostAsJsonAsync<Feature>(targetUri, feature);
                        postTask.Wait();

                        // Process the result
                        if (postTask.Result.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Succesfully published feature '{0}' to group {1} for branch {2}.",
                                              feature.Title,
                                              _options.GroupName ?? "Default",
                                              _options.BranchName);

                        }
                        else
                        {
                            Console.Error.WriteLine("Publishing feature '{0}' to uri '{1}' resulted in statuscode '{2}'",
                                                    feature.Title,
                                                    targetUri,
                                                    postTask.Result.StatusCode);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        private string GetGroupUri()
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "{0}/api/features/{1}/{2}",
                                 _options.AugurkUrl.TrimEnd('/'),
                                 _options.BranchName,
                                 _options.GroupName ?? "Default");
        }

        private string GetFeatureUri(string groupUri, string featureTitle)
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "{0}/{1}",
                                 groupUri,
                                 featureTitle);
        }
    }
}
