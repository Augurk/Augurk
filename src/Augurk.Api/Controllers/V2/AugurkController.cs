/*
 Copyright 2017, Augurk
 
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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Augurk.Entities;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving and persisting Augurk settings.
    /// </summary>
    [RoutePrefix("api/v2")]
    public class AugurkController : ApiController
    {
        private readonly CustomizationManager _customizationManager = new CustomizationManager();

        /// <summary>
        /// Gets the customization settings.
        /// </summary>
        /// <returns>All customization related settings and their values.</returns>
        [Route("customization")]
        [HttpGet]
        public async Task<Customization> GetCustomizationAsync()
        {
            return await _customizationManager.GetOrCreateCustomizationSettingsAsync();
        }

        /// <summary>
        /// Pesists the provided customization settings.
        /// </summary>
        [Route("customization")]
        [HttpPut]
        [HttpPost]
        public async Task PersistCustomizationAsync(Customization customizationSettings)
        {
            await _customizationManager.PersistCustomizationSettingsAsync(customizationSettings);
        }
    }
}
