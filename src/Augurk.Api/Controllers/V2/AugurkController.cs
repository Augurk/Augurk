/*
 Copyright 2017-2018, Augurk
 
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
using System.Web.Http.Description;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving and persisting Augurk settings.
    /// </summary>
    [RoutePrefix("api/v2")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AugurkController : ApiController
    {
        private readonly CustomizationManager _customizationManager = new CustomizationManager();
        private readonly ConfigurationManager _configurationManager = new ConfigurationManager();

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

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>All configuration.</returns>
        [Route("configuration")]
        [HttpGet]
        public async Task<Configuration> GetConfigurationAsync()
        {
            return await _configurationManager.GetOrCreateConfigurationAsync();
        }

        /// <summary>
        /// Pesists the provided configurations.
        /// </summary>
        [Route("configuration")]
        [HttpPut]
        [HttpPost]
        public async Task PersisConfigurationAsync(Configuration configuration)
        {
            await _configurationManager.PersistConfigurationAsync(configuration);
        }
    }
}
