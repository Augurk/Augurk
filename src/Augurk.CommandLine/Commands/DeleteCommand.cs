using Augurk.CommandLine.Options;
using Augurk.CommandLine.Plumbing;
using System;
using System.ComponentModel.Composition;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the delete command.
    /// </summary>
    [Export(typeof(ICommand))]
    [ExportMetadata("Verb", DeleteOptions.VERB_NAME)]
    internal class DeleteCommand : ICommand
    {
        private readonly DeleteOptions _options;

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="options">A <see cref="DeleteOptions"/> instance containing the relevant options for the command.</param>
        [ImportingConstructor]
        public DeleteCommand(DeleteOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Called when the command is to be executed.
        /// </summary>
        public void Execute()
        {
            // Determine the base Uri we're going to perform the delete on
            var baseUri = new Uri($"{_options.AugurkUrl}/api/v2/products/{_options.ProductName}/");
            var deleteUri = baseUri;

            // If a group name is specified
            if (!String.IsNullOrWhiteSpace(_options.GroupName))
            {
                // Append it to the base uri
                deleteUri = new Uri(deleteUri, $"groups/{_options.GroupName}/");
            }

            // If a feature name is specified
            if (!String.IsNullOrWhiteSpace(_options.FeatureName))
            {
                // Make sure that the group name is also specified
                if (String.IsNullOrWhiteSpace(_options.GroupName))
                {
                    Console.WriteLine("When deleting a specific feature a group name that the feature belongs to must also be specified.");
                    return;
                }

                // Append the feature name to the base uri
                deleteUri = new Uri(deleteUri, $"features/{_options.FeatureName}/");
            }

            // If a version is specified
            if (!String.IsNullOrWhiteSpace(_options.Version))
            {
                // Append the version to the base uri
                deleteUri = new Uri(deleteUri, $"versions/{_options.Version}/");
            }

            // Perform the delete operation
            using (var client = AugurkHttpClientFactory.CreateHttpClient(_options))
            {
                // Call the URL
                var response = client.DeleteAsync(deleteUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    if (!String.IsNullOrWhiteSpace(_options.FeatureName))
                    {
                        Console.WriteLine($"Succesfully deleted feature {_options.FeatureName} from Augurk at {_options.AugurkUrl}");
                    }
                    else
                    {
                        Console.WriteLine($"Succesfully deleted features from Augurk at {_options.AugurkUrl}");
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(_options.FeatureName))
                    {
                        Console.WriteLine($"Deleting feature {_options.FeatureName} from Augurk at {_options.AugurkUrl} failed with statuscode {response.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine($"Deleting features from Augurk at {_options.AugurkUrl} failed with statuscode {response.StatusCode}");
                    }
                }
            }
        }
    }
}
