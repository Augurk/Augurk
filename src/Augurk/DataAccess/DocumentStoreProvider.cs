// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Augurk.Api.Managers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Expiration;
using Raven.Embedded;

namespace Augurk
{
    /// <summary>
    /// Provides access to the <see cref="IDocumentStore" />.
    /// </summary>
    public class DocumentStoreProvider : IDocumentStoreProvider, IHostedService
    {
        /// <summary>
        /// Raven DB License data model.
        /// </summary>
        public class RavenDBLicense
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the keys.
            /// </summary>
            public string[] Keys { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DocumentStoreProvider> _logger;
        private readonly ILogger<MigrationManager> _migrationLogger;

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        public DocumentStoreProvider(IConfiguration configuration, IWebHostEnvironment environment, ILogger<DocumentStoreProvider> logger, ILogger<MigrationManager> migrationLogger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
            var currentBinariesDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Build the options for the server
            var dotNetVersion = Environment.Version.ToString();
            _logger.LogInformation("Configuring embedded RavenDb server to use version {DotNetVersion} of .NET.", dotNetVersion);
            var serverOptions = new ServerOptions
            {
                AcceptEula = true,
                DataDirectory = Path.Combine(currentBinariesDirectory, "data"),
                FrameworkVersion = dotNetVersion,
            };

            // Setup logging for RavenDB during development
            if (_environment.IsDevelopment())
            {
                // Put down license.json from secrets if not found
                var licensePath = Path.Combine(currentBinariesDirectory, "RavenDBServer", "license.json");
                if (!File.Exists(licensePath))
                {
                    var license = new RavenDBLicense();
                    _configuration.Bind("license", license);
                    await File.WriteAllTextAsync(licensePath, JsonSerializer.Serialize(license, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
                }

                // Enable diagnostic logging
                serverOptions.LogsPath = Path.Combine(currentBinariesDirectory, "logs");
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

        private void RavenDbServerProcessesExited(object sender, ServerProcessExitedEventArgs args)
        {
            _logger.LogError("Embedded RavenDb server has exited unexpectedly.");
        }
    }
}
