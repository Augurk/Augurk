/*
 Copyright 2019, Augurk

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
using Augurk;
using Augurk.Api.Managers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods for registering various services with depdendency injection.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Registers the necessary services needed for RavenDB.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection" /> to add the services to.</param>
        public static void AddRavenDb(this IServiceCollection services)
        {
            // NOTE: Using TryAddSingleton here to allow integration tests to plug in a different IDocumentStoreProvider
            services.TryAddSingleton<IDocumentStoreProvider, DocumentStoreProvider>();
        }

        /// <summary>
        /// Registers all the necessary managers needed by the Augurk application.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection" /> to add the services to.</param>
        public static void AddManagers(this IServiceCollection services)
        {
            services.AddSingleton<IExpirationManager, ExpirationManager>();
            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
            services.AddSingleton<ICustomizationManager, CustomizationManager>();

            services.AddSingleton<IProductManager, ProductManager>();
            services.AddSingleton<IFeatureManager, FeatureManager>();

            services.AddSingleton<IAnalysisReportManager, AnalysisReportManager>();
            services.AddSingleton<IDependencyManager, DependencyManager>();
        }
    }
}
