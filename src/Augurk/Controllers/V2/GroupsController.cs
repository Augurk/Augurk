/*
 Copyright 2017-2019, Augurk
 
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
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available groups of related features within a product.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/products/{productName}/groups")]
    public class GroupsController : Controller
    {
        private readonly IFeatureManager _featureManager;

        public GroupsController(IFeatureManager featureManager)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        /// <summary>
        /// Gets all the groups for the provided <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the groups for.</param>
        /// <returns>Returns a range of groups.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<Group>> GetGroupsAsync(string productName)
        {
            return await _featureManager.GetGroupedFeatureDescriptionsAsync(productName);
        }

        /// <summary>
        /// Deletes all versions of the provided group from the database.
        /// </summary>
        /// <param name="productName">Name of the product that the group belongs to.</param>
        /// <param name="groupName">Name of the group that should be deleted.</param>
        [Route("{groupName}")]
        [HttpDelete]
        public async Task DeleteGroupAsync(string productName, string groupName)
        {
            await _featureManager.DeleteFeaturesAsync(productName, groupName);
        }

        /// <summary>
        /// Deletes the provided version of the provided group from the database.
        /// </summary>
        /// <param name="productName">Name of the product that the group belongs to.</param>
        /// <param name="groupName">Name of the group that should be deleted.</param>
        /// <param name="version">The version of the group that should be deleted.</param>
        [Route("{groupName}/versions/{version}")]
        [HttpDelete]
        public async Task DeleteGroupAsync(string productName, string groupName, string version)
        {
            await _featureManager.DeleteFeaturesAsync(productName, groupName, version);
        }
    }
}
