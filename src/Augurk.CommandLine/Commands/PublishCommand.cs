using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using Augurk.CommandLine.Options;
using Augurk.Entities;
using TechTalk.SpecFlow.Parser;
using System.ComponentModel.Composition;
using Augurk.CommandLine.Plumbing;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the publish command.
    /// </summary>
    [Export(typeof(ICommand))]
    [ExportMetadata("Verb", PublishOptions.VERB_NAME)]
    internal class PublishCommand : ICommand
    {
        private readonly PublishOptions _options;

        [ImportingConstructor]
        public PublishCommand(PublishOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            // Determine the version of the API we're going to use
            if (string.IsNullOrWhiteSpace(_options.ProductName))
            {
                // Execute the command by using V1 of the API
                ExecuteUsingV1Api();
            }
            else
            {
                // Execute the command by using V2 of the API
                ExecuteUsingV2Api();
            }
        }

        private void ExecuteUsingV1Api()
        {
            // Instantiate a new parser, using the provided language
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo(_options.Language ?? "en-US"));

            using (var client = AugurkHttpClientFactory.CreateHttpClient(_options))
            {
                // Get the base uri for all further operations
                string groupUri = $"{_options.AugurkUrl.TrimEnd('/')}/api/features/{_options.BranchName}/{_options.GroupName ?? "Default"}";

                // Clear any existing features in this group, if required
                if (_options.ClearGroup)
                {
                    Console.WriteLine($"Clearing existing features in group {_options.GroupName ?? "Default"} for branch {_options.BranchName}.");
                    client.DeleteAsync(groupUri).Wait();
                }

                // Parse and publish each of the provided feature files
                var expandedList = Expand(_options.FeatureFiles);
                foreach (var featureFile in expandedList)
                {
                    try
                    {
                        using (TextReader reader = File.OpenText(featureFile))
                        {
                            // Parse the feature and convert it to the correct format
                            Feature feature = parser.Parse(reader, featureFile).ConvertToFeature();

                            // Get the uri to which the feature should be published
                            string targetUri = $"{groupUri}/{feature.Title}";

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
        }

        /// <summary>
        /// Expands a list of feature file specifications by resolving wildcards or getting all
        /// .feature files when it's a directory.
        /// </summary>
        /// <remarks>
        /// Wildcard characters only work for file specifications, not for directories.
        /// Specifications with * or ? in the directory specification will be ignored.
        /// </remarks>
        /// <param name="featureFiles">List of feature files specified by the user.</param>
        /// <returns>Expanded set of file names.</returns>
        private static IEnumerable<string> Expand(IEnumerable<string> featureFiles)
        {
            var expandedList = new List<string>();

            foreach (var fileSpec in featureFiles)
            {
                if (Directory.Exists(fileSpec))
                {
                    // spec is a directory, automatically expand to *.feature
                    var files = Directory.GetFiles(fileSpec, "*.feature");
                    expandedList.AddRange(files);
                    continue;
                }

                if (fileSpec.Contains('?') || fileSpec.Contains('*'))
                {
                    // resolve wildcard in file spec
                    var directory = Path.GetDirectoryName(fileSpec);
                    var spec = Path.GetFileName(fileSpec);

                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        if (!string.IsNullOrEmpty(spec))
                        {
                            var files = Directory.GetFiles(directory, spec);
                            expandedList.AddRange(files);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping invalid directory specification '{directory}'.");
                    }
                    continue;
                }

                if (File.Exists(fileSpec))
                {
                    expandedList.Add(fileSpec);
                }
                else
                {
                    Console.WriteLine($"Skipping file '{fileSpec}' because it does not exist.");
                }
            }

            return expandedList;
        }

        private void ExecuteUsingV2Api()
        {
            // Instantiate a new parser, using the provided language
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo(_options.Language ?? "en-US"));

            using (var client = AugurkHttpClientFactory.CreateHttpClient(_options))
            {
                // Get the base uri for all further operations
                string groupUri = $"{_options.AugurkUrl.TrimEnd('/')}/api/v2/products/{_options.ProductName}/groups/{_options.GroupName}/features";

                // Parse and publish each of the provided feature files
                var expandedList = Expand(_options.FeatureFiles);
                foreach (var featureFile in expandedList)
                {
                    try
                    {
                        using (TextReader reader = File.OpenText(featureFile))
                        {
                            // Parse the feature and convert it to the correct format
                            Feature feature = parser.Parse(reader, featureFile).ConvertToFeature();

                            // Get the uri to which the feature should be published
                            string targetUri = $"{groupUri}/{feature.Title}/versions/{_options.Version}/";

                            // Publish the feature
                            var postTask = client.PostAsJsonAsync<Feature>(targetUri, feature);
                            postTask.Wait();

                            // Process the result
                            if (postTask.Result.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Succesfully published feature '{feature.Title}' version '{_options.Version}' for product '{_options.ProductName}' to group '{_options.GroupName}'.");
                            }
                            else
                            {
                                Console.Error.WriteLine($"Publishing feature '{feature.Title}' version '{_options.Version}' to uri '{targetUri}' resulted in statuscode '{postTask.Result.StatusCode}'");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
