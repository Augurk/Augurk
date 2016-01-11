using CommandLine;

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

        /// <summary>
        /// Flag to indicate that the tool must run under integrated security to access the Augurk API's.
        /// </summary>
        [Option("useIntegratedSecurity", HelpText = "Use integrated security to access the Augurk API's. Do not specify username and password when using integrated security", MutuallyExclusiveSet = "username,password,useBasicAuthentication", Required = false)]
        public bool UseIntegratedSecurity { get; set; }

        /// <summary>
        /// Flag to indicate that the tool must use basic HTTP authentication to access the Augurk API's.
        /// </summary>
        [Option("useBasicAuthentication", HelpText = "Use basic HTTP authentication to access the Augurk API's. You must also specify a username and a password.", MutuallyExclusiveSet = "useIntegratedSecurity", Required = false)]
        public bool UseBasicAuthentication { get; set; }

        /// <summary>
        /// Username for basic HTTP authentication against the Augurk API's.
        /// </summary>
        [Option("username", HelpText = "Username for basic authentication against the Augurk API's.", Required = false)]
        public string BasicAuthenticationUsername { get; set; }

        /// <summary>
        /// Password for basic HTTP authentication against the Augurk API's.
        /// </summary>
        [Option("password", HelpText = "Password for basic authentication against the Augurk API's.", Required = false)]
        public string BasicAuthenticationPassword { get; set; }
    }
}
