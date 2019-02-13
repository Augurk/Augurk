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
    /// Provides methods to persist and retrieve configuration from storage.
    /// </summary>
    public class ConfigurationManager
    {
        private const string KEY = "urn:Augurk:Configuration";
        private readonly IDocumentStoreProvider _storeProvider;

        public ConfigurationManager(IDocumentStoreProvider documentStoreProvider)
        {
            _storeProvider = documentStoreProvider ?? throw new ArgumentNullException(nameof(documentStoreProvider));
        }

        /// <summary>
        /// Retrieves the configuration; or, creates it if it does not exist.
        /// </summary>
        /// <returns>A <see cref="Configuration"/> instance containing the configuration.</returns>
        public async Task<Configuration> GetOrCreateConfigurationAsync()
        {
            Configuration configuration = null;

            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                configuration = await session.LoadAsync<Configuration>(KEY);
            }

            if (configuration == null)
            {
                configuration = Defaults.Configuration;
                await PersistConfigurationAsync(configuration);
            }

            return configuration;
        }

        /// <summary>
        /// Persists the provided configuration.
        /// </summary>
        /// <param name="configuration">A <see cref="Configuration"/> instance containing the configuration that should be persisted.</param>
        public async Task PersistConfigurationAsync(Configuration configuration)
        {
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                // Using the store method when the configuration already exists in the database will override it completely, this is acceptable
                await session.StoreAsync(configuration, KEY);
                await session.SaveChangesAsync();
            }
        }
    }
}
