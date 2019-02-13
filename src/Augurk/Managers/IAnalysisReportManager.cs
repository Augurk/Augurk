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