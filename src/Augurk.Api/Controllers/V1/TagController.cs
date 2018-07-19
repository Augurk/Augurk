/*
 Copyright 2015-2018, Augurk
 
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
using System.Web.Http;
using System.Web.Http.Description;
using Augurk.Api.Managers;
using Augurk.Entities;

namespace Augurk.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TagController : ApiController
    {
        private readonly ProductManager _productManager = new ProductManager();
        private readonly FeatureManager _featureManager = new FeatureManager();

        [Route("api/tags/{branchName}")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync(string branchName)
        {
            return FeatureProcessor.RemoveServerTags(await _productManager.GetTagsAsync(branchName));
        }

        [Route("api/tags/{branchName}/{tag}/features")]
        [HttpGet]
        public async Task<IEnumerable<FeatureDescription>> GetFeaturesAsync(string branchName, string tag)
        {
            return await _featureManager.GetFeatureDescriptionsByBranchAndTagAsync(branchName, tag);
        }
    }
}