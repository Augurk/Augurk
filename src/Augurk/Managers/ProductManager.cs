/*
 Copyright 2014-2020, Augurk

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
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api.Indeces;
using System;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Microsoft.Extensions.Logging;

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
        /// <returns>Returns a range of productName names.</returns>
        public async Task<IEnumerable<string>> GetProductsAsync()
        {
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                var result = await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                    .OrderBy(feature => feature.Product)
                                    .Select(feature => feature.Product)
                                    .Distinct()
                                    .ToListAsync();

                _logger.LogInformation("Found {ProductCount} products", result.Count);
                return result;
            }
        }

        /// <summary>
        /// Gets the description of the provided product.
        /// </summary>
        /// <param name="productName">The name of the product for which the description should be retrieved.</param>
        /// <returns>The description of the requested product; or, null.</returns>
        public async Task<string> GetProductDescriptionAsync(string productName)
        {
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                return await session.Query<DbProduct, Products_ByName>()
                                    .Where(product => product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                                    .Select(product => product.DescriptionMarkdown)
                                    .SingleOrDefaultAsync();
            }
        }

        /// <summary>
        /// Inserts or updates the provided description for the product with the provided name.
        /// </summary>
        /// <param name="productName">The name of the product for which the description should be persisted.</param>
        /// <param name="descriptionMarkdown">The description that should be persisted.</param>
        public async Task InsertOrUpdateProductDescriptionAsync(string productName, string descriptionMarkdown)
        {
            var dbProduct = new DbProduct()
            {
                Name = productName,
                DescriptionMarkdown = descriptionMarkdown
            };

            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                // Using the store method when the product already exists in the database will override it completely, this is acceptable
                await session.StoreAsync(dbProduct, dbProduct.GetIdentifier());
                await session.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes the specified productName.
        /// </summary>
        /// <param name="productName">The name of the product to delete.</param>
        public async Task DeleteProductAsync(string productName)
        {
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.Operations.Send(
                    new DeleteByQueryOperation<DbFeature, Features_ByTitleProductAndGroup>(x =>
                        x.Product == productName))
                    .WaitForCompletionAsync();
            }
        }

        /// <summary>
        /// Deletes a specified version of the specified productName.
        /// </summary>
        /// <param name="productName">The productName to delete.</param>
        /// <param name="version">The version of the productName to delete.</param>
        public async Task DeleteProductAsync(string productName, string version)
        {
            throw new NotImplementedException();

            /*using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.Operations.Send(
                    new DeleteByQueryOperation<DbFeature, Features_ByTitleProductAndGroup>(x =>
                        x.Product == productName && x.Version == version))
                    .WaitForCompletionAsync();
            }*/
        }

        /// <summary>
        /// Gets all available tags for the provided <paramref name="productName">productName</paramref>.
        /// </summary>
        /// <param name="productName">Name of the productName to get the available tags for.</param>
        /// <returns>Returns a range of tags for the provided <paramref name="productName">productName</paramref>.</returns>
        public async Task<IEnumerable<string>> GetTagsAsync(string productName)
        {
            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
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
}
