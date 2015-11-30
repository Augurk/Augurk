using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Augurk.CommandLine.Options
{
    /// <summary>
    /// Command line options that are shared between all verbs.
    /// </summary>
    internal class SharedOptions
    {
        /// <summary>
        /// Gets or sets the URL of the instance of Augurk to which the features files should be published.
        /// </summary>
        [Option("url", HelpText = "URL to the Augurk Instance to which the features files should be published.", Required = true)]
        public string AugurkUrl { get; set; }
    }
}
