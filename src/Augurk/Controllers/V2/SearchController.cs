// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Search;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/>.
        /// </summary>
        public SearchController(IFeatureManager featureManager)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        /// <summary>
        /// Searches for items matching the searchquery.
        /// </summary>
        /// <param name="query">The query on which the search should be based.</param>
        /// <returns>Returns a range of <see cref="FeatureDescription"/> instance describing the features.</returns>
        [Route("")]
        [HttpGet]
        public async Task<SearchResults> GetFeaturesForProductAndGroupAsync([FromQuery(Name = "q")] string query)
        {
            // Search for features
            var featureMatches = await _featureManager.Search(query);

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
