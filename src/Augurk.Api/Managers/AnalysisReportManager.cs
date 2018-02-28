/*
 Copyright 2018, Augurk
 
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
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Raven.Json.Linq;
using Augurk.Entities.Analysis;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve analysis reports from storage.
    /// </summary>
    public class AnalysisReportManager
    {
        /// <summary>
        /// Gets or sets the JsonSerializerSettings that should be used when (de)serializing.
        /// </summary>
        internal static JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Gets or sets the configuration manager which should be used by this instance.
        /// </summary>
        private ConfigurationManager ConfigurationManager { get; set; }

        public AnalysisReportManager()
            : this(new ConfigurationManager())
        {
               
        }

        internal AnalysisReportManager(ConfigurationManager configurationManager)
        {
            ConfigurationManager = configurationManager;
        }

        public async Task InsertOrUpdateAnalysisReportAsync(AnalysisReport report, string productName, string version)
        {
            var configuration = await ConfigurationManager.GetOrCreateConfigurationAsync();

            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                // Store will override the existing report if it already exists
                await session.StoreAsync(report, $"{productName}/{version}/{report.AnalyzedProject}");

                session.SetExpirationIfEnabled(report, version, configuration);

                await session.SaveChangesAsync();
            }
        }
    }
}