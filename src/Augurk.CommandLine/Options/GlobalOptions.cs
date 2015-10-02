using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace Augurk.CommandLine.Options
{
    /// <summary>
    /// Represents the available command line options.
    /// </summary>
    internal class GlobalOptions
    {
        /// <summary>
        /// Options when publishing features.
        /// </summary>
        [VerbOption(PublishOptions.VERB_NAME, HelpText = "Publish features to Augurk.")]
        public PublishOptions PublishVerb { get; set; }

        /// <summary>
        /// Options when deleting features.
        /// </summary>
        [VerbOption(DeleteOptions.VERB_NAME, HelpText = "Delete features from Augurk.")]
        public DeleteOptions DeleteVerb { get; set; }

        /// <summary>
        /// Gets the usage of the command line tool for the specified verb.
        /// </summary>
        /// <param name="verb">Name of the verb for which the usage should be retrieved.</param>
        /// <returns>Returns a string containing the usage of the specified verb.</returns>
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}
