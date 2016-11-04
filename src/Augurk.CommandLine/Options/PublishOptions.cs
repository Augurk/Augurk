using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Augurk.CommandLine.Options
{
    /// <summary>
    /// Represents the available command line options when publishing features.
    /// </summary>
    internal class PublishOptions : SharedOptions
    {
        /// <summary>
        /// Name of the verb for this set of options.
        /// </summary>
        public const string VERB_NAME = "publish";

        /// <summary>
        /// Gets or sets the set of comma separated .feature file or directory specifications that should be published to Augurk.
        /// </summary>
        [OptionList("featureFiles", Separator = ',', HelpText = "Comma separated set of feature file or directory specifications that should be published to Augurk. Also supports * or ? wildcard characters for file specifications and directory names (will automatically search for *.feature within that directory)", Required = true)]
        public IEnumerable<string> FeatureFiles { get; set; }

        /// <summary>
        /// Gets or sets the name of the product under which the feature files should be published.
        /// </summary>
        [Option("productName", HelpText = "Name of the product under which the feature files should be published. Cannot be used in combination with the --branchName option.", MutuallyExclusiveSet = "product/branch", Required = false)]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group under which the feature files should be published.
        /// </summary>
        [Option("groupName", HelpText = "Name of the group under which the feature files should be published.", Required = true)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the branch under which the feature files should be published.
        /// </summary>
        [Option("branchName", HelpText = "Name of the branch under which the feature files should be published. Cannot be used in combination with the --productName option.", MutuallyExclusiveSet = "product/branch", Required = false)]
        public string BranchName { get; set; }

        /// <summary>
        /// Gets or sets the version of the feature files that are being published.
        /// </summary>
        [Option("version", HelpText = "Version of the feature files that should be published. Cannot be used in combination with the --clearGroup option.", MutuallyExclusiveSet = "version", Required = false, DefaultValue = "0.0.0")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the group should be cleared prior to publishing the new features.
        /// </summary>
        [Option("clearGroup", HelpText = "If set the group specified by --groupName will be cleared prior to publishing the new features. Cannot be used in combination with the --version option.", MutuallyExclusiveSet = "version", Required = false)]
        public bool ClearGroup { get; set; }

        /// <summary>
        /// Gets or sets the language in which the feature files have been written.
        /// </summary>
        [Option("language", HelpText = "Language in which the features files have been written. For example: `en-US` or `nl-NL`.", DefaultValue = "en-US", Required = false)]
        public string Language { get; set; }
    }
}
