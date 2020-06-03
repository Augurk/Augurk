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
using System.Threading.Tasks;
using Augurk.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Linq;
using System.Collections.Generic;
using Raven.Client.Documents;
using Raven.Client.Documents.Smuggler;
using Microsoft.AspNetCore.Http;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving and persisting Augurk settings.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AugurkController : Controller
    {
        private readonly ICustomizationManager _customizationManager;
        private readonly IConfigurationManager _configurationManager;
        private readonly IFeatureManager _featureManager;
        private readonly IExpirationManager _expirationManager;
        private readonly IDocumentStore _documentStore;
        private readonly MigrationManager _migrationManager;

        public AugurkController(ICustomizationManager customizationManager,
                                IConfigurationManager configurationManager,
                                IFeatureManager featureManager,
                                IExpirationManager expirationManager,
                                IDocumentStoreProvider storeProvider,
                                MigrationManager migrationManager)
        {
            _customizationManager = customizationManager ?? throw new ArgumentNullException(nameof(customizationManager));
            _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _expirationManager = expirationManager ?? throw new ArgumentNullException(nameof(expirationManager));
            _documentStore = storeProvider?.Store ?? throw new ArgumentNullException(nameof(storeProvider));
            _migrationManager = migrationManager ?? throw new ArgumentNullException(nameof(migrationManager));
        }

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
        public async Task PersistCustomizationAsync([FromBody]Customization customizationSettings)
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
        public async Task PersistConfigurationAsync([FromBody]Configuration configuration)
        {
            await _configurationManager.PersistConfigurationAsync(configuration);
        }

        /// <summary>
        /// Imports existing data into Augurk.
        /// </summary>
        [Route("import")]
        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = Int32.MaxValue, ValueLengthLimit = Int32.MaxValue)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Import(IFormFile file)
        {
            // Make sure that we have an input file
            if (file == null)
            {
                return BadRequest("No file to import specified.");
            }

            // Store the uploaded file into a temporary location
            string filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                // Copy file to temporary location
                await file.CopyToAsync(stream);

                // Perform import
                var importOptions = new DatabaseSmugglerImportOptions()
                {
                    OperateOnTypes = DatabaseItemType.Documents,
                    IncludeExpired = false,
                };

                var operation = await _documentStore.Smuggler.ImportAsync(importOptions, filePath);
                await operation.WaitForCompletionAsync();

                // Delete temporary file
                System.IO.File.Delete(filePath);

                // Set the expirations as configured
                // TODO Restore functionality
                // await _expirationManager.ApplyExpirationPolicyAsync(await _configurationManager.GetOrCreateConfigurationAsync());

                // Migrate the imported data asynchronously
                var taskWeShallNotWaitFor = _migrationManager.StartMigrating();

                return NoContent();
            }
        }

        /// <summary>
        /// Exports data in Augurk to a file that can be used to import the data into another instance.
        /// </summary>
        [Route("export")]
        [HttpGet]
        public async Task<ActionResult> Export()
        {
            // Setup an export using RavenDb's Smuggler API
            var exportTimestamp = DateTime.Now;
            var fileName = $"augurk-{exportTimestamp.ToString("yyyy-dd-M-HHmmss")}.bak";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);

            // Setup the export options
            var exportOptions = new DatabaseSmugglerExportOptions
            {
                OperateOnTypes = DatabaseItemType.Documents,
                IncludeExpired = false,
                Collections = new List<string>
                {
                    "DbFeatures",
                    "AnalysisReports",
                    "DbProducts",
                }
            };

            // Perform the export
            var operation = await _documentStore.Smuggler.ExportAsync(exportOptions, filePath);
            await operation.WaitForCompletionAsync();

            // Stream the backup back to the client
            return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", fileName);
        }
    }
}
