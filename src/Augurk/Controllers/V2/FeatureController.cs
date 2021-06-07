// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Api.Managers;
using Augurk.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available features.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/products/{productName}/groups/{groupName}/features")]
    public class FeatureController : Controller
    {
        private readonly IFeatureManager _featureManager;
        private readonly Analyzer _analyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureController"/>.
        /// </summary>
        public FeatureController(IFeatureManager featureManager, IAnalysisReportManager analysisReportManager)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _analyzer = new Analyzer(_featureManager, analysisReportManager);
        }

        /// <summary>
        /// Gets the available features.
        /// </summary>
        /// <param name="productName">Name of the product to which the feature belongs.</param>
        /// <param name="groupName">Name of the group to which the feature belongs.</param>
        /// <returns>Returns a range of <see cref="FeatureDescription"/> instance describing the features.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<FeatureDescription>> GetFeaturesForProductAndGroupAsync(string productName, string groupName)
        {
            return await _featureManager.GetFeatureDescriptionsByProductAndGroupAsync(productName, groupName);
        }

        /// <summary>
        /// Gets the available versions of a feature.
        /// </summary>
        /// <param name="productName">Name of the product to which the feature belongs.</param>
        /// <param name="groupName">Name of the group to which the feature belongs.</param>
        /// <param name="featureTitle">Title of the feature to get the available versions for.</param>
        /// <returns>Returns a range of versions available for the requested feature.</returns>
        [Route("{featureTitle}/versions")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetFeatureVersions(string productName, string groupName, string featureTitle)
        {
            return await _featureManager.GetFeatureAvailableVersions(productName, groupName, featureTitle);
        }

        /// <summary>
        /// Gets a specific feature.
        /// </summary>
        /// <param name="productName">Name of the product to which the feature belongs.</param>
        /// <param name="groupName">Name of the group to which the feature belongs.</param>
        /// <param name="featureTitle">Title of the feature.</param>
        /// <param name="version">Version of the feature to get.</param>
        /// <returns>Returns a <see cref="DisplayableFeature"/>.</returns>
        [Route("{featureTitle}/versions/{version}", Name = "GetFeatureV2")]
        [HttpGet]
        public async Task<DisplayableFeature> GetFeatureAsync(string productName, string groupName, string featureTitle, string version)
        {
            return await _featureManager.GetFeatureAsync(productName, groupName, featureTitle, version);
        }

        /// <summary>
        /// Saves a new feature into the database.
        /// </summary>
        /// <param name="feature">A <see cref="Feature"/> instance to save.</param>
        /// <param name="productName">Name of the product that the feature belongs to.</param>
        /// <param name="groupName">Name of the group that the feature belongs to.</param>
        /// <param name="title">Title of the feature.</param>
        /// <param name="version">Version of feature.</param>
        /// <returns>Returns a reponse message indicating whether saving the feature succeeded.</returns>
        [Route("{title}/versions/{version}")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(PostResult), 202)]
        public async Task<ActionResult<PostResult>> PostAsync([FromBody] Feature feature, string productName, string groupName, string title, string version)
        {
            if (!feature.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("The title provided by the POST data and the title in uri do not match!");
            }

            var dbFeature = await _featureManager.InsertOrUpdateFeatureAsync(feature, productName, groupName, version);

            // Run the analysis for this feature
            if (dbFeature != null)
            {
                try
                {
                    await _analyzer.AnalyzeAndPeristResultsAsync(productName, version, dbFeature);
                }
                catch
                {
                    // Don't do anything, a broken analysis is no reason to report an error
                }
            }

            // Build a nice result
            var result = new PostResult
            {
                ApiUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}",
                WebUrl = $"{Request.Scheme}://{Request.Host}/preview/feature/{productName}/{groupName}/{title}/{version}"
            };

            return Accepted(result);
        }

        /// <summary>
        /// Deletes all versions of a feature from the database.
        /// </summary>
        /// <param name="productName">Name of the product that the feature belongs to.</param>
        /// <param name="groupName">Name of the group that the feature belongs to.</param>
        /// <param name="title">Title of the feature to delete.</param>
        /// <returns>Returns whether deleting the feature was succesful or not.</returns>
        [Route("{title}/")]
        [HttpDelete]
        public async Task DeleteAsync(string productName, string groupName, string title)
        {
            await _featureManager.DeleteFeatureAsync(productName, groupName, title);
        }

        /// <summary>
        /// Deletes a version of a feature from the database.
        /// </summary>
        /// <param name="productName">Name of the product that the feature belongs to.</param>
        /// <param name="groupName">Name of the group that the feature belongs to.</param>
        /// <param name="title">Title of the feature to delete.</param>
        /// <param name="version">Version of the feature to delete.</param>
        /// <returns>Returns whether deleting the feature was succesful or not.</returns>
        [Route("{title}/versions/{version}")]
        [HttpDelete]
        public async Task DeleteAsync(string productName, string groupName, string title, string version)
        {
            await _featureManager.DeleteFeatureAsync(productName, groupName, title, version);
        }
    }
}
