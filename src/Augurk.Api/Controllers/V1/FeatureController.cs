/*
 Copyright 2014-2015, Mark Taling
 
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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Test;

namespace Augurk.Api.Controllers.V1
{
    public class FeatureController : ApiController
    {
        private readonly FeatureManager _featureManager = new FeatureManager();

        [Route("api/features/{branchName}")]
        [HttpGet]
        public async Task<IEnumerable<Group>> GetAsync(string branchName)
        {
            return await _featureManager.GetGroupedFeatureDescriptionsAsync(branchName);
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpGet]
        public async Task<DisplayableFeature> GetAsync(string branchName, string groupName, string title)
        {
            // Get the feature from storage
            DisplayableFeature feature = await _featureManager.GetFeatureAsync(branchName, groupName, title);

            return feature;
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(Feature feature, string branchName, string groupName, string title)
        {
            if (!feature.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The title provided by the POST data and the title in uri do not match!");
            }

            var response = Request.CreateResponse(HttpStatusCode.Created);

            try
            {
                await _featureManager.InsertOrUpdateFeatureAsync(feature, branchName, groupName);
            }
            catch (Exception exception)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }

            return response;
        }

        [Obsolete]
        [Route("api/features/{branchName}/{groupName}")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(Feature feature, string branchName, string groupName)
        {
            return await PostAsync(feature, branchName, groupName, feature.Title);
        }

        [Route("api/features/{branchName}/{groupName}/{title}/testresult")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(FeatureTestResult testResult, string branchName, string groupName, string title)
        {
            if (!testResult.FeatureTitle.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The title provided by the POST data and the title in uri do not match!");
            }

            var response = Request.CreateResponse(HttpStatusCode.Created);

            try
            {
                await _featureManager.PersistFeatureTestResultAsync(testResult, branchName, groupName);
            }
            catch (Exception exception)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }

            return response;
        }

        [Route("api/features/{branchName}")]
        [HttpDelete]
        public async Task DeleteAsync(string branchName)
        {
            await _featureManager.DeleteFeaturesAsync(branchName);
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
            await _featureManager.DeleteFeatureAsync(branchName, groupName, title);
        }
    }
}