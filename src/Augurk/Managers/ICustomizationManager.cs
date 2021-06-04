// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    public interface ICustomizationManager
    {
        /// <summary>
        /// Retrieves the customization settings; or, creates them if they do not exist.
        /// </summary>
        /// <returns>A <see cref="Customization"/> instance containing the customization settings.</returns>
        Task<Customization> GetOrCreateCustomizationSettingsAsync();

        /// <summary>
        /// Persists the provided customization settings.
        /// </summary>
        /// <param name="customizationSettings">A <see cref="Customization"/> instance containing the settings that should be persisted.</param>
        Task PersistCustomizationSettingsAsync(Customization customizationSettings);
    }
}
