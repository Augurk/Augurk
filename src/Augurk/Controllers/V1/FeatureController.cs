﻿// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Test;
using Microsoft.AspNetCore.Mvc;

namespace Augurk.Api.Controllers
{
    [ApiVersion("1.0")]
    public class FeatureController : Controller
    {
        private const string UNKNOWN_VERSION = "0.0.0";
        private readonly IFeatureManager _featureManager;
        private readonly IProductManager _productManager;

        public FeatureController(IFeatureManager featureManager, IProductManager productManager)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
        }

        [Route("api/features/{branchName}")]
        [HttpGet]
        public async Task<IEnumerable<Group>> GetAsync(string branchName)
        {
            // NOTE: Using the branchName as product name because of backwards compatability
            return await _featureManager.GetGroupedFeatureDescriptionsAsync(branchName);
        }

        [Route("api/features/{branchName}/{groupName}/{title}", Name = "GetFeature")]
        [HttpGet]
        public async Task<DisplayableFeature> GetAsync(string branchName, string groupName, string title)
        {
            // Get the feature from storage
            // NOTE: Using the branchName as product name because of backwards compatability
            var feature = await _featureManager.GetFeatureAsync(branchName, groupName, title, UNKNOWN_VERSION);

            return feature;
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<ActionResult<Feature>> PostAsync(Feature feature, string branchName, string groupName, string title)
        {
            if (!feature.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("The title provided by the POST data and the title in uri do not match!");
            }

            // NOTE: Using the branchName as product name because of backwards compatability
            await _featureManager.InsertOrUpdateFeatureAsync(feature, branchName, groupName, UNKNOWN_VERSION);
            return CreatedAtRoute("GetFeature", feature);
        }

        [Obsolete("This API should no longer be used")]
        [Route("api/features/{branchName}/{groupName}")]
        [HttpPost]
        public async Task<ActionResult<Feature>> PostAsync([FromBody] Feature feature, string branchName, string groupName)
        {
            return await PostAsync(feature, branchName, groupName, feature.Title);
        }

        [Route("api/features/{branchName}/{groupName}/{title}/testresult")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(201)]
        public async Task<ActionResult<FeatureTestResult>> PostAsync([FromBody] FeatureTestResult testResult, string branchName, string groupName, string title)
        {
            if (!testResult.FeatureTitle.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("The title provided by the POST data and the title in uri do not match!");
            }

            await _featureManager.PersistFeatureTestResultAsync(testResult, branchName, groupName, UNKNOWN_VERSION);
            return CreatedAtRoute("GetFeature", null);
        }

        [Route("api/features/{branchName}")]
        [HttpDelete]
        public async Task DeleteAsync(string branchName)
        {
            // In V2 this has become part of the ProductsController (formerly: Branchcontroller).
            // In order to minimize the duplication of code, use the new controller.
            await _productManager.DeleteProductAsync(branchName);
        }

        [Route("api/features/{branchName}/{groupName}")]
        [HttpDelete]
        public async Task DeleteAsync(string branchName, string groupName)
        {
            await _featureManager.DeleteFeaturesAsync(branchName, groupName);
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpDelete]
        public async Task DeleteAsync(string branchName, string groupName, string title)
        {
            await _featureManager.DeleteFeatureAsync(branchName, groupName, title, UNKNOWN_VERSION);
        }
    }
}
