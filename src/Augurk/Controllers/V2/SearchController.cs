/*
 Copyright 2020, Augurk

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
using Augurk.Entities;
using Augurk.Entities.Search;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available features.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/search")]
    public class SearchController : Controller
    {
        private readonly IFeatureManager _featureManager;
        private readonly Analyzer _analyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/>.
        /// </summary>
        public SearchController(IFeatureManager featureManager, IAnalysisReportManager analysisReportManager)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _analyzer = new Analyzer(_featureManager, analysisReportManager);
        }

        /// <summary>
        /// Searches for items matching the searchquery.
        /// </summary>
        /// <param name="query">The query on which the search should be based.</param>
        /// <returns>Returns a range of <see cref="FeatureDescription"/> instance describing the features.</returns>
        [Route("")]
        [HttpGet]
        public async Task<SearchResults> GetFeaturesForProductAndGroupAsync([FromQuery(Name = "q")]string query)
        {
            // Search for features
            var featureMatches =  await _featureManager.Search(query);

            // Prepare the results
            var results = new SearchResults
            {
                SearchQuery = query,
                FeatureMatches = featureMatches.ToList()
            };

            return results;
        }
    }
}
