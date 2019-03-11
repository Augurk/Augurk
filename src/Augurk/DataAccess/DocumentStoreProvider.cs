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
using System;
using System.IO;
using System.Reflection;
using Augurk.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Embedded;

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
        public DocumentStoreProvider(IHostingEnvironment environment, ILogger<DocumentStoreProvider> logger)
        {
            // Build the options for the server
            string dotNetCoreVersion = EnvironmentUtils.GetNetCoreVersion();
            logger.LogInformation("Configuring embedded RavenDb server to use version {NetCoreVersion} of .NET Core.", dotNetCoreVersion);
            var serverOptions = new ServerOptions
            {
                DataDirectory = Path.Combine(Environment.CurrentDirectory, "data"),
                FrameworkVersion = dotNetCoreVersion
            };

            // Build the options for the database
            var databaseOptions = new DatabaseOptions("AugurkStore")
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