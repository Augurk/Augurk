// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Augurk.Entities.Analysis;
using System.Collections.Generic;
using System.Linq;
using Augurk.Api.Indeces.Analysis;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents;
using System;
using Microsoft.Extensions.Logging;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve analysis reports from storage.
    /// </summary>
    public class AnalysisReportManager : IAnalysisReportManager
    {
        private readonly IDocumentStoreProvider _storeProvider;
        private readonly IConfigurationManager _configurationManager;
        private readonly ILogger<AnalysisReportManager> _logger;

        public AnalysisReportManager(IDocumentStoreProvider storeProvider, IConfigurationManager configurationManager, ILogger<AnalysisReportManager> logger)
        {
            _storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Inserts or updates the provided <paramref name="report"/> for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product that the analysis report relates to.</param>
        /// <param name="version">Version of the product that the analysis report relates to.</param>
        /// <param name="report">An <see cref="AnalysisReport"/> to insert or update.</param>
        public async Task InsertOrUpdateAnalysisReportAsync(string productName, string version, AnalysisReport report)
        {
            var configuration = await _configurationManager.GetOrCreateConfigurationAsync();

            // Store will override the existing report if it already exists
            using var session = _storeProvider.Store.OpenAsyncSession();
            await session.StoreAsync(report, $"{productName}/{version}/{report.AnalyzedProject}");

            session.SetExpirationAccordingToConfiguration(report, version, configuration);
            session.Advanced.GetMetadataFor(report)["Product"] = productName;

            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all the available <see cref="AnalysisReports"/> stored for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product to get the analysis reports for.</param>
        /// <param name="version">Version of the product to get the analysis reports for.</param>
        /// <returns>A range of <see cref="AnalysisReport"/> instances stored for the provided product and version.</returns>
        public IEnumerable<AnalysisReport> GetAnalysisReportsByProductAndVersionAsync(string productName, string version)
        {
            using var session = _storeProvider.Store.OpenSession();
            return session.Query<AnalysisReports_ByProductAndVersion.Entry, AnalysisReports_ByProductAndVersion>()
                       .Where(report => report.Product == productName && report.Version == version)
                       .OfType<AnalysisReport>()
                       .ToList();
        }

        /// <summary>
        /// Gets all the invocations stored in the database.
        /// </summary>
        /// <returns>Returns a range of <see cref="DbInvocation" /> instances representing the invocations stored in the database.</returns>
        public async Task<IEnumerable<DbInvocation>> GetAllDbInvocations()
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var result = await session.Query<DbInvocation>().ToListAsync();
            _logger.LogInformation("Retrieved {InvocationCount} invocations", result.Count);
            return result;
        }

        /// <summary>
        /// Persists the provided range of <paramref name="invocations"/> for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product to store the invocations for.</param>
        /// <param name="version">Version of the product to store the invocations for.</param>
        /// <param name="invocations">A range of <see cref="DbInvocation"/> instances representing the invocations to persist.</param>
        public async Task PersistDbInvocationsAsync(string productName, string version, IEnumerable<DbInvocation> invocations)
        {
            var configuration = await _configurationManager.GetOrCreateConfigurationAsync();

            using var session = _storeProvider.Store.OpenAsyncSession();
            foreach (var invocation in invocations)
            {
                await session.StoreAsync(invocation, $"{productName}/{version}/{invocation.Signature}");
                session.SetExpirationAccordingToConfiguration(invocation, version, configuration);
                var metadata = session.Advanced.GetMetadataFor(invocation);
                metadata["Product"] = productName;
                metadata["Version"] = version;
            }

            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes all stored invocations for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product to delete the invocations for.</param>
        /// <param name="version">Version of the product to delete the invocations for.</param>
        public async Task DeleteDbInvocationsAsync(string productName, string version)
        {
            await _storeProvider.Store.Operations.Send(
                new DeleteByQueryOperation<Invocation_ByProductAndVersion.Entry, Invocation_ByProductAndVersion>(
                    x => x.Product == productName && x.Version == version))
                .WaitForCompletionAsync();
        }
    }
}
