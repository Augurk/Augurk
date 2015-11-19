using CommandLine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Augurk.CommandLine.Options
{
    /// <summary>
    /// Represents the available command line options when deleting features.
    /// </summary>
    internal class DeleteOptions : SharedOptions
    {
        /// <summary>
        /// Name of the verb for this set of options
        /// </summary>
        public const string VERB_NAME = "delete";

        /// <summary>
        /// Gets or sets the name of the product under which the feature files should be published.
        /// </summary>
        [Option("productName", HelpText = "Name of the product containing the features to delete.", Required = true)]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group under which the feature files should be published.
        /// </summary>
        [Option("groupName", HelpText = "Name of the group containing the features to delete.", Required = false)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group under which the feature files should be published.
        /// </summary>
        [Option("featureName", HelpText = "Name of the feature to delete.", Required = false)]
        public string FeatureName { get; set; }

        /// <summary>
        /// Gets or sets the version of the feature files that are being published.
        /// </summary>
        [Option("version", HelpText = "Version of the feature(s) to delete.", Required = false)]
        public string Version { get; set; }
    }
}
