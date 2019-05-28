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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Augurk.Entities;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;
using Sparrow.Json;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve configuration from storage.
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        private const string DOCUMENT_KEY = "urn:Augurk:Configuration";
        private readonly IDocumentStoreProvider _storeProvider;
        private readonly IExpirationManager _expirationManager;

        public ConfigurationManager(IDocumentStoreProvider documentStoreProvider, IExpirationManager expirationManager)
        {
            _storeProvider = documentStoreProvider ?? throw new ArgumentNullException(nameof(documentStoreProvider));
            _expirationManager = expirationManager ?? throw new ArgumentNullException(nameof(expirationManager));
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
                configuration = await session.LoadAsync<Configuration>(DOCUMENT_KEY);
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
            Configuration originalConfiguration = null;
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                // Retrieve the original configuration
                originalConfiguration = await session.LoadAsync<Configuration>(DOCUMENT_KEY);

                // Using the store method when the configuration already exists in the database will override it completely, this is acceptable
                await session.StoreAsync(configuration, DOCUMENT_KEY);


                await session.SaveChangesAsync();
            }

            if(originalConfiguration == null 
                || originalConfiguration.ExpirationDays != configuration.ExpirationDays
                || originalConfiguration.ExpirationEnabled != configuration.ExpirationEnabled
                || !originalConfiguration.ExpirationRegex.Equals(configuration.ExpirationRegex, StringComparison.Ordinal)
                )
            {
                await _expirationManager.ApplyExpirationPolicyAsync(configuration);
            }
        }
    }
}
