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
using Augurk.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Augurk.Api.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/dependencies")]
    public class DependencyController : Controller
    {
        private readonly IDependencyManager _dependencyManager;

        public DependencyController(IDependencyManager dependencyManager)
        {
            _dependencyManager = dependencyManager ?? throw new ArgumentNullException(nameof(dependencyManager));
        }

        [Route("")]
        [HttpGet]
        public Task<IEnumerable<FeatureGraph>> GetSystemWideDependencies()
        {
            return _dependencyManager.GetTopLevelFeatureGraphsAsync();
        }

        [Route("products/{productName}/features/{featureName}/versions/{version}")]
        [HttpGet]
        public Task<FeatureGraph> GetFeatureGraph(string productName, string featureName, string version)
        {
            return _dependencyManager.GetFeatureGraphAsync(productName, featureName, version);
        }
    }
}
