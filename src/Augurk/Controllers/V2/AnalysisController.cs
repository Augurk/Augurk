/*
 Copyright 2018-2019, Augurk
 
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
using Augurk.Api.Managers;
using Augurk.Entities.Analysis;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for publishing analysis results.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/products/{productName}/versions/{productVersion}/analysis")]
    public class AnalysisController : Controller
    {
        private readonly IAnalysisReportManager _analysisReportManager;
        private readonly Analyzer _analyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisController"/>.
        /// </summary>
        public AnalysisController(IAnalysisReportManager analysisReportManager, IFeatureManager featureManager)
        {
            _analysisReportManager = analysisReportManager ?? throw new ArgumentNullException(nameof(analysisReportManager));
            _analyzer = new Analyzer(featureManager, _analysisReportManager);
        }

        /// <summary>
        /// Persists an analysis report for further analysis.
        /// </summary>
        /// <param name="analysisReport">The report as producted by one of the Augurk automation analyzers.</param>
        /// <param name="productName">Name of the product that the feature belongs to.</param>
        /// <param name="groupName">Name of the group that the feature belongs to.</param>
        [Route("reports")]
        [HttpPost]
        public async Task<ActionResult> PostAnalysisReport([FromBody]AnalysisReport analysisReport, string productName, string version)
        {
            analysisReport.Version = version;

            await _analysisReportManager.InsertOrUpdateAnalysisReportAsync(productName, version, analysisReport);

            // Run the analysis
            await _analyzer.AnalyzeAndPersistResultsAsync(productName, version);

            return Accepted();
        }
    }
}
