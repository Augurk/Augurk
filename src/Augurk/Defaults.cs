// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.
using Augurk.Entities;

namespace Augurk.Api
{
    /// <summary>
    /// Provides default values for items which should otherwise be configured in the database.
    /// </summary>
    internal static class Defaults
    {
        /// <summary>
        /// Gets the default customization.
        /// </summary>
        /// <remarks>
        /// Each call will result a new instance being returned.
        /// </remarks>
        public static Customization Customization =>
            new()
            {
                InstanceName = "Augurk",
            };

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        /// <remarks>
        /// Each call will result a new instance being returned.
        /// </remarks>
        public static Configuration Configuration =>
            new()
            {
                ExpirationEnabled = false,
                ExpirationDays = 30,
                ExpirationRegex = "[0-9.]+-.*",
            };
    }
}
