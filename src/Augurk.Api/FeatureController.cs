/*
 Copyright 2014, Mark Taling
 
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
using System.Web.Http;
using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Test;

namespace Augurk.Api
{
    public class FeatureController : ApiController
    {
        private readonly FeatureManager _featureManager = new FeatureManager();

        [Route("api/features/{branchName}")]
        [HttpGet]
        public IEnumerable<Group> Get(string branchName)
        {
            return _featureManager.GetGroupedFeatureDescriptions(branchName);
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpGet]
        public DisplayableFeature Get(string branchName, string groupName, string title)
        {
            try
            {
                // Get the feature from storage
                DisplayableFeature feature = _featureManager.GetFeature(branchName, groupName, title);

                // Process the server tags
                if (feature != null)
                {
                    var processor = new FeatureProcessor();
                    processor.Process(feature);
                }

                return feature;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpPost]
        public HttpResponseMessage Post(Feature feature, string branchName, string groupName, string title)
        {
            if (!feature.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The title provided by the POST data and the title in uri do not match!");
            }

            var response = Request.CreateResponse(HttpStatusCode.Created);

            try
            {
                _featureManager.InsertOrUpdateFeature(feature, branchName, groupName);
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
        public HttpResponseMessage Post(Feature feature, string branchName, string groupName)
        {
            return Post(feature, branchName, groupName, feature.Title);
        }

        [Route("api/features/{branchName}/{groupName}/{title}/testresult")]
        [HttpPost]
        public HttpResponseMessage Post(FeatureTestResult testResult, string branchName, string groupName, string title)
        {
            if (!testResult.FeatureTitle.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The title provided by the POST data and the title in uri do not match!");
            }

            var response = Request.CreateResponse(HttpStatusCode.Created);

            try
            {
                _featureManager.PersistFeatureTestResult(testResult, branchName, groupName);
            }
            catch (Exception exception)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
            return response;
        }

        [Route("api/features/{branchName}")]
        [HttpDelete]
        public void Delete(string branchName)
        {
            _featureManager.DeleteFeatures(branchName);
        }

        [Route("api/features/{branchName}/{groupName}")]
        [HttpDelete]
        public void Delete(string branchName, string groupName)
        {
            _featureManager.DeleteFeatures(branchName, groupName);
        }

        [Route("api/features/{branchName}/{groupName}/{title}")]
        [HttpDelete]
        public void Delete(string branchName, string groupName, string title)
        {
            _featureManager.DeleteFeature(branchName, title);
        }
    }
}