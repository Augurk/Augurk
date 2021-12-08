// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api.Indeces;
using Raven.Client.Documents;
using Microsoft.Extensions.Logging;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve features from storage.
    /// </summary>
    public class MigrationManager
    {
        private readonly IDocumentStoreProvider _storeProvider;
        private readonly ILogger<MigrationManager> _logger;

        public MigrationManager(IDocumentStoreProvider storeProvider, ILogger<MigrationManager> logger)
        {
            _storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// An asynchonous method that starts the migration of any features that are not yet hashed.
        /// </summary>
        public async Task StartMigrating()
        {
            // Migrate the Products
            await MigrateProducts();

            // This migration should be slightly rewritten to make sure it is only handled once
            dynamic features;
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                features = await session.Query<DbFeature, Features_WithoutHash>()
                                        .Statistics(out var stats)
                                        .Select(f => new { f.Product, f.Group, f.Title })
                                        .ToListAsync();

                if (stats.IsStale)
                {
                    // The index is stale, it's probably better to wait a bit.
                    var newTask = Task.Delay(60000).ContinueWith(t => StartMigrating());
                    return;
                }

            };

            foreach (var feature in features)
            {
                // Don't await them, let them run in parallel
                MigrateFeature(feature.Product, feature.Group, feature.Title);
            }
        }

        /// <summary>
        /// Migrates the feature to the new structure.
        /// </summary>
        /// <param name="productName">Name of the product the feature belongs to.</param>
        /// <param name="groupName">Name of the group the product belongs to.</param>
        /// <param name="title">Title of the feature to migrate.</param>
        public async Task MigrateFeature(string productName, string groupName, string title)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var originalFeatures = await session.Query<DbFeature>()
                                                .Where(feature => feature.Product == productName
                                                            && feature.Group == groupName
                                                            && feature.Title == title)
                                                .ToListAsync();

            // A check whether it is neccesary to migrate it would be nice

            var newFeatures = from feature in originalFeatures
                              group feature by feature.CalculateHash()
                                into groupedFeature
                              let first = groupedFeature.First()
                              select new DbFeature(first, first.Product, first.Group, first.ParentTitle)
                              {
                                  Hash = groupedFeature.Key,
                                  Versions = groupedFeature.SelectMany(f => f.Versions ?? new[] { f.Version }).ToArray()
                              };

            _logger.LogInformation($"Migrating {title} ({productName}/{groupName}) from {originalFeatures.Count} versioned features to {newFeatures.Count()} unversioned features.");

            foreach (var newFeature in newFeatures)
            {
                await session.StoreAsync(newFeature, newFeature.GetIdentifier());
            }

            foreach (var oldFeature in originalFeatures)
            {
                session.Delete(oldFeature);
            }

            await session.SaveChangesAsync();
        }

        public async Task MigrateProducts()
        {
            var session = _storeProvider.Store.OpenAsyncSession();

            DbMigrations migrations = await session.LoadAsync<DbMigrations>(DbMigrations.ID) ?? new();

            // Only start the migration if it hasn't been completed before
            if(!migrations.ProductFromFeatures)
            {
                var productsByFeature = await session.Query<DbFeature>() 
                                                     .Select(feature => feature.Product)
                                                     .ToListAsync();

                var actualProducts = await session.Query<DbProduct>()
                                                  .Select(product => product.Name)
                                                  .ToListAsync();

                // Determine the missing products
                var missingProducts = productsByFeature.Except(actualProducts).ToList();

                if(missingProducts.Count > 0)
                {
                    foreach(var productName in missingProducts)
                    {
                        System.Console.WriteLine($"Adding product {productName}");
                        var product = new DbProduct {Name = productName};
                        await session.StoreAsync(product, product.GetIdentifier());
                    }
                }
                
                migrations.ProductFromFeatures = true;
                await session.StoreAsync(migrations, DbMigrations.ID);

                await session.SaveChangesAsync();

            }       
        }
    }
}
