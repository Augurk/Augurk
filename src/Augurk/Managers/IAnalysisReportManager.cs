// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Entities.Analysis;

namespace Augurk.Api.Managers
{
    public interface IAnalysisReportManager
    {
        /// <summary>
        /// Inserts or updates the provided <paramref name="report"/> for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product that the analysis report relates to.</param>
        /// <param name="version">Version of the product that the analysis report relates to.</param>
        /// <param name="report">An <see cref="AnalysisReport"/> to insert or update.</param>
        Task InsertOrUpdateAnalysisReportAsync(string productName, string version, AnalysisReport report);

        /// <summary>
        /// Gets all the available <see cref="AnalysisReports"/> stored for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product to get the analysis reports for.</param>
        /// <param name="version">Version of the product to get the analysis reports for.</param>
        /// <returns>A range of <see cref="AnalysisReport"/> instances stored for the provided product and version.</returns>
        IEnumerable<AnalysisReport> GetAnalysisReportsByProductAndVersionAsync(string productName, string version);

        /// <summary>
        /// Gets all the invocations stored in the database.
        /// </summary>
        /// <returns>Returns a range of <see cref="DbInvocation" /> instances representing the invocations stored in the database.</returns>
        Task<IEnumerable<DbInvocation>> GetAllDbInvocations();

        /// <summary>
        /// Persists the provided range of <paramref name="invocations"/> for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product to store the invocations for.</param>
        /// <param name="version">Version of the product to store the invocations for.</param>
        /// <param name="invocations">A range of <see cref="DbInvocation"/> instances representing the invocations to persist.</param>
        Task PersistDbInvocationsAsync(string productName, string version, IEnumerable<DbInvocation> invocations);

        /// <summary>
        /// Deletes all stored invocations for the provided <paramref name="productName">product</paramref> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">Name of the product to delete the invocations for.</param>
        /// <param name="version">Version of the product to delete the invocations for.</param>
        Task DeleteDbInvocationsAsync(string productName, string version);
    }
}
