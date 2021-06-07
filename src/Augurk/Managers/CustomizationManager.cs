// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve customization settings from storage.
    /// </summary>
    public class CustomizationManager : ICustomizationManager
    {
        private const string DOCUMENT_KEY = "urn:Augurk:Customization";
        private readonly IDocumentStoreProvider _storeProvider;

        public CustomizationManager(IDocumentStoreProvider storeProvider)
        {
            _storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
        }

        /// <summary>
        /// Retrieves the customization settings; or, creates them if they do not exist.
        /// </summary>
        /// <returns>A <see cref="Customization"/> instance containing the customization settings.</returns>
        public async Task<Customization> GetOrCreateCustomizationSettingsAsync()
        {
            Customization customizationSettings = null;

            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                customizationSettings = await session.LoadAsync<Customization>(DOCUMENT_KEY);
            }

            if (customizationSettings == null)
            {
                customizationSettings = Defaults.Customization;
                await PersistCustomizationSettingsAsync(customizationSettings);
            }

            return customizationSettings;
        }

        /// <summary>
        /// Persists the provided customization settings.
        /// </summary>
        /// <param name="customizationSettings">A <see cref="Customization"/> instance containing the settings that should be persisted.</param>
        public async Task PersistCustomizationSettingsAsync(Customization customizationSettings)
        {
            // Using the store method when the customization already exists in the database will override it completely, this is acceptable
            using var session = _storeProvider.Store.OpenAsyncSession();
            await session.StoreAsync(customizationSettings, DOCUMENT_KEY);
            await session.SaveChangesAsync();
        }
    }
}
