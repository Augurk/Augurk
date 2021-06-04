// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// Retrieves the configuration; or, creates it if it does not exist.
        /// </summary>
        /// <returns>A <see cref="Configuration"/> instance containing the configuration.</returns>
        Task<Configuration> GetOrCreateConfigurationAsync();

        /// <summary>
        /// Persists the provided configuration.
        /// </summary>
        /// <param name="configuration">A <see cref="Configuration"/> instance containing the configuration that should be persisted.</param>
        Task PersistConfigurationAsync(Configuration configuration);
    }
}
