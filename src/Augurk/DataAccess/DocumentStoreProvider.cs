/*
 Copyright 2018, Augurk

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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Configuration;
using Raven.Client.ServerWide;
using Raven.Embedded;
using static Raven.Client.Documents.Operations.Configuration.StudioConfiguration;

namespace Augurk
{
    /// <summary>
    /// Provides access to the <see cref="IDocumentStore" />.
    /// </summary>
    public class DocumentStoreProvider : IDocumentStoreProvider
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public DocumentStoreProvider(IHostingEnvironment environment)
        {
            // Create the necessary options
            var serverOptions = new ServerOptions { DataDirectory = Path.Combine(Environment.CurrentDirectory, "data") };
            var databaseRecord = new DatabaseRecord
            {
                DatabaseName = "FeatureStore",
                Studio = new StudioConfiguration
                {
                    Environment = environment.IsDevelopment() ? StudioEnvironment.Development : StudioEnvironment.Production
                },
            };

            var databaseOptions = new DatabaseOptions(databaseRecord)
            {
                Conventions = new DocumentConventions
                {
                    IdentityPartsSeparator = "-"
                }
            };

            // Start the mebedded RavenDB server
            EmbeddedServer.Instance.StartServer(serverOptions);
            Store = EmbeddedServer.Instance.GetDocumentStore(databaseOptions);

            // Make sure that indexes are created
            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), Store);

            // Check if we're running in development
            if (environment.IsDevelopment())
            {
                // Open the RavenDB studio
                EmbeddedServer.Instance.OpenStudioInBrowser();
            }
        }

        /// <summary>
        /// Gets the <see cref="IDocumentStore" /> instance used to access the underlying data store.
        /// </summary>
        public IDocumentStore Store { get; }
    }
}