// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
            var options = new QueryOperationOptions() { AllowStale = true };

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
                var operation = await _storeProvider.Store.Operations.SendAsync(new PatchByQueryOperation(new IndexQuery() { Query = query }, options));

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
                var secondOperation = await _storeProvider.Store.Operations.SendAsync(new PatchByQueryOperation(new IndexQuery() { Query = query }, options));

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
                var operation = await _storeProvider.Store.Operations.SendAsync(new PatchByQueryOperation(new IndexQuery() { Query = query }, options));
                await operation.WaitForCompletionAsync();
            }
        }

    }
}
