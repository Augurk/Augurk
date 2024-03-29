﻿// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk;
using Augurk.Api.Managers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
            services.TryAddSingleton<DocumentStoreProvider>();
            services.TryAddSingleton<IDocumentStoreProvider>(sp => sp.GetRequiredService<DocumentStoreProvider>());
            services.TryAddSingleton<IHostedService>(sp => sp.GetRequiredService<DocumentStoreProvider>());
        }

        /// <summary>
        /// Registers all the necessary managers needed by the Augurk application.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection" /> to add the services to.</param>
        public static void AddManagers(this IServiceCollection services)
        {
            services.AddSingleton<MigrationManager>();

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
