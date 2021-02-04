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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Expiration;
using Raven.Embedded;
using Augurk.Api.Managers;
using System.Threading;
using System.Threading.Tasks;

namespace Augurk
{
    /// <summary>
    /// Provides access to the <see cref="IDocumentStore" />.
    /// </summary>
    public class DocumentStoreProvider : IDocumentStoreProvider, IHostedService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DocumentStoreProvider> _logger;
        private readonly ILogger<MigrationManager> _migrationLogger;

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public DocumentStoreProvider(IWebHostEnvironment environment, ILogger<DocumentStoreProvider> logger, ILogger<MigrationManager> migrationLogger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _migrationLogger = migrationLogger ?? throw new ArgumentNullException(nameof(migrationLogger));
        }

        /// <summary>
        /// Gets the <see cref="IDocumentStore" /> instance used to access the underlying data store.
        /// </summary>
        public IDocumentStore Store { get; private set; }

        /// <summary>
        /// Called when Augurk starts up.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> that is triggered when startup is cancelled.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Build the options for the server
            var dotNetVersion = Environment.Version.ToString();
            _logger.LogInformation("Configuring embedded RavenDb server to use version {DotNetVersion} of .NET.", dotNetVersion);
            var serverOptions = new ServerOptions
            {
                AcceptEula = true,
                DataDirectory = Path.Combine(Environment.CurrentDirectory, "data"),
                FrameworkVersion = dotNetVersion,
            };

            // Setup logging for RavenDB during development
            if (_environment.IsDevelopment())
            {
                // Enable diagnostic logging
                serverOptions.LogsPath = Path.Combine(Environment.CurrentDirectory, "logs");
                serverOptions.CommandLineArgs.Add("--Logs.Mode=Information");
            }

            // Build the options for the database
            var databaseOptions = new DatabaseOptions("AugurkStore")
            {
                Conventions = new DocumentConventions
                {
                    IdentityPartsSeparator = '-'
                }
            };

            // Start the mebedded RavenDB server
            EmbeddedServer.Instance.ServerProcessExited += RavenDbServerProcessesExited;
            EmbeddedServer.Instance.StartServer(serverOptions);
            Store = await EmbeddedServer.Instance.GetDocumentStoreAsync(databaseOptions, cancellationToken);

            // Make sure that indexes are created
            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), Store);

            // Enable the expiration option, even if it is already enabled
            // This runs async, there is no need to wait on it
            await Store.Maintenance.SendAsync(new ConfigureExpirationOperation(new ExpirationConfiguration
            {
                Disabled = false,
                DeleteFrequencyInSec = 60
            }), cancellationToken);

            // Start asynchronous migration
            // Note: We're instantiating the MigrationManager here directly, rather than having it injected
            //       This is because of a chicken-egg problem, since the MigrationManager also needs a
            //       IDocumentStoreProvider
            await new MigrationManager(this, _migrationLogger).StartMigrating();

            // Write the RavenDB Studio URL to the log
            var ravenUrl = await EmbeddedServer.Instance.GetServerUriAsync(cancellationToken);
            _logger.LogInformation("RavenDB has started successfully and is available on {RavenDbURL}", ravenUrl);
        }

        /// <summary>
        /// Called when Augurk stops.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> that is triggered when stop is cancelled.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Dispose the store
            EmbeddedServer.Instance.ServerProcessExited -= RavenDbServerProcessesExited;
            Store.Dispose();
            return Task.CompletedTask;
        }

        private void RavenDbServerProcessesExited(object sender, ServerProcessExitedEventArgs args) => _logger.LogError("Embedded RavenDb server has exited unexpectedly.");
    }
}