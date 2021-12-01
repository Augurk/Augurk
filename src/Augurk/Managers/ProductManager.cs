// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api.Indeces;
using System;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Microsoft.Extensions.Logging;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods for retrieving products from storage.
    /// </summary>
    public class ProductManager : IProductManager
    {
        private readonly IDocumentStoreProvider _storeProvider;
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(IDocumentStoreProvider storeProvider, ILogger<ProductManager> logger)
        {
            _storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>Returns a collection containing all products.</returns>
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var result = await session.Query<DbProduct>()
                                .ToListAsync();

            _logger.LogInformation("Found {ProductCount} products", result.Count);
            return result.Select(product => new Product {
                                    Name = product.Name,
                                    DisplayName = product.DisplayName ?? product.Name,
                                    ShortDescription = product.ShortDescriptionMarkdown,
                                    Description = product.DescriptionMarkdown
                                 }).OrderBy(product => product.DisplayName);
        }

        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>Returns a range of productName names.</returns>
        public async Task<IEnumerable<string>> GetProductTitlesAsync()
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var result = await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                .OrderBy(feature => feature.Product)
                                .Select(feature => feature.Product)
                                .Distinct()
                                .ToListAsync();

            _logger.LogInformation("Found {ProductCount} products", result.Count);
            return result;
        }

        /// <summary>
        /// Gets the description of the provided product.
        /// </summary>
        /// <param name="productName">The name of the product for which the description should be retrieved.</param>
        /// <returns>The description of the requested product; or, null.</returns>
        public async Task<string> GetProductDescriptionAsync(string productName)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            return await session.Query<DbProduct, Products_ByName>()
                                .Where(product => product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                                .Select(product => product.DescriptionMarkdown)
                                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Inserts or updates the provided description for the product with the provided name.
        /// </summary>
        /// <param name="productName">The name of the product for which the description should be persisted.</param>
        /// <param name="descriptionMarkdown">The description that should be persisted.</param>
        public async Task InsertOrUpdateProductDescriptionAsync(string productName, string descriptionMarkdown)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var dbProduct = await session.LoadAsync<DbProduct>(DbProductExtensions.GetIdentifier(productName));
            
            if(dbProduct != null)
            {
                // There is no point in updating the productName, as it is the identifier we used to find this document.
                dbProduct.DescriptionMarkdown = descriptionMarkdown;
            }
            else
            {
                dbProduct = new DbProduct()
                {
                    Name = productName,
                    DescriptionMarkdown = descriptionMarkdown
                };
                
                await session.StoreAsync(dbProduct, dbProduct.GetIdentifier());
            }
        
            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the specified productName.
        /// </summary>
        /// <param name="productName">The name of the product to delete.</param>
        public async Task DeleteProductAsync(string productName)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            await session.Advanced.DocumentStore.Operations.Send(
                new DeleteByQueryOperation<DbFeature, Features_ByTitleProductAndGroup>(x =>
                    x.Product == productName))
                .WaitForCompletionAsync();
        }

        /// <summary>
        /// Deletes a specified version of the specified productName.
        /// </summary>
        /// <param name="productName">The productName to delete.</param>
        /// <param name="version">The version of the productName to delete.</param>
        public async Task DeleteProductAsync(string productName, string version)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var dbFeatures = await session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                          .Where(f => f.Product == productName
                                                   && f.Version == version)
                                           .OfType<DbFeature>()
                                           .ToListAsync();

            foreach (var dbFeature in dbFeatures)
            {
                if (dbFeature.Versions.Length == 1)
                {
                    // Remove the feature as this is the only version
                    session.Delete(dbFeature);
                }
                else
                {
                    // Remove this version, but let the feature remain as it contains other versions
                    var versions = dbFeature.Versions.ToList();
                    versions.Remove(version);
                    dbFeature.Versions = versions.ToArray();
                }
            }

            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all available tags for the provided <paramref name="productName">productName</paramref>.
        /// </summary>
        /// <param name="productName">Name of the productName to get the available tags for.</param>
        /// <returns>Returns a range of tags for the provided <paramref name="productName">productName</paramref>.</returns>
        public async Task<IEnumerable<string>> GetTagsAsync(string productName)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            return await session.Query<Features_ByProductAndBranch.TaggedFeature, Features_ByProductAndBranch>()
                                .Where(feature => feature.Product.Equals(productName, StringComparison.CurrentCultureIgnoreCase))
                                .OrderBy(feature => feature.Tag)
                                .Select(feature => feature.Tag)
                                .Distinct()
                                .Take(512)
                                .ToListAsync();
        }
    }
}
