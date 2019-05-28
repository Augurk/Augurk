/*
 Copyright 2019, Augurk
 
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

using System.Threading.Tasks;
using System;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Operations;
using Augurk.Entities;
using System.Web;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods for expiring documents.
    /// </summary>
    public class ExpirationManager : IExpirationManager
    {
        private readonly IDocumentStoreProvider _storeProvider;
        public ExpirationManager(IDocumentStoreProvider storeProvider)
        {
            _storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
        }

        public async Task ApplyExpirationPolicyAsync(Configuration configuration)
        {
            if (configuration.ExpirationEnabled)
            {
                // Set the expiration on all records that have a matching version
                var query = $@"from @all_docs as d
                                where regex(d.Version, '{HttpUtility.JavaScriptStringEncode(configuration.ExpirationRegex)}')
                                update
                                {{
                                    if(!this[""@metadata""][""upload-date""]) 
                                    {{
                                        this[""@metadata""][""upload-date""] = new Date(lastModified(d));
                                    }}

                                    var expirationDate = new Date(this[""@metadata""][""upload-date""]);
                                    expirationDate.setDate(expirationDate.getDate() + {configuration.ExpirationDays});
                                    this[""@metadata""][""@expires""] = expirationDate; 
                                    put(id(d), this);
                                }}";
                var operation = await _storeProvider.Store.Operations.SendAsync(new PatchByQueryOperation(new IndexQuery() { Query = query }));

                // Remove the expiration from all versioned records that do not have a matching version
                query = $@"from @all_docs as d
                            where exists(d.Version) AND NOT regex(d.Version, '{HttpUtility.JavaScriptStringEncode(configuration.ExpirationRegex)}')
                            update
                            {{
                                if(!this[""@metadata""][""upload-date""]) 
                                {{
                                    this[""@metadata""][""upload-date""] = new Date(lastModified(d));
                                }}

                                delete this[""@metadata""][""@expires""]; 
                                put(id(d), this);
                            }}";
                var secondOperation = await _storeProvider.Store.Operations.SendAsync(new PatchByQueryOperation(new IndexQuery() { Query = query }));

                await operation.WaitForCompletionAsync();
                await secondOperation.WaitForCompletionAsync();
            }
            else
            {
                // Remove the expiration from all versioned records
                var query = $@"from @all_docs as d
                                where exists(d.Version)
                                update
                                {{
                                    if(!this[""@metadata""][""upload-date""]) 
                                    {{
                                        this[""@metadata""][""upload-date""] = new Date(lastModified(d));
                                    }}

                                    delete this[""@metadata""][""@expires""]; 
                                    put(id(d), this);
                                }}";
                var operation = await _storeProvider.Store.Operations.SendAsync(new PatchByQueryOperation(new IndexQuery() { Query = query }));
                await operation.WaitForCompletionAsync();
            }
        }

    }
}