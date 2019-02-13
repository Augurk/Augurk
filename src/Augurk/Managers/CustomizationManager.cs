/*
 Copyright 2017-2019, Augurk
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
using System.Threading.Tasks;
using Augurk.Entities;
using Raven.Client.Documents;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve customization settings from storage.
    /// </summary>
    public class CustomizationManager
    {
        private const string KEY = "urn:Augurk:Customization";
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
                customizationSettings = await session.LoadAsync<Customization>(KEY);
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
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                // Using the store method when the customization already exists in the database will override it completely, this is acceptable
                await session.StoreAsync(customizationSettings, KEY);
                await session.SaveChangesAsync();
            }
        }
    }
}
