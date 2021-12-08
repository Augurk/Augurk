// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Entities
{
    /// <summary>
    /// Contains the Augurk configuration.
    /// </summary>
    public class Configuration
    {
        #region Expiration
        /// <summary>
        /// Indicates whether the expiration of features is enabled.
        /// </summary>
        public bool ExpirationEnabled { get; set; }

        /// <summary>
        /// Defines the number of days after which an features should expire.
        /// </summary>
        public int ExpirationDays { get; set; }

        /// <summary>
        /// Defines the regular expression which a feature version must match to expire.
        /// </summary>
        public string ExpirationRegex { get; set; }
        #endregion

        #region Dependencies
        /// <summary>
        /// Indicates whether the analysis of dependencies is enabled.
        /// </summary>
        public bool DependenciesEnabled { get; set; }
        #endregion
    }
}
