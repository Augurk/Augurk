/*
 Copyright 2014-2015, Mark Taling
 
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
using Raven.Client;
using Raven.Client.Linq;
using Augurk.Api.Indeces;
using System;
using Raven.Abstractions.Data;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods for retrieving products from storage.
    /// </summary>
    public class ProductManager
    {
        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>Returns a range of product names.</returns>
        public async Task<IEnumerable<string>> GetProductsAsync()
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                return await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                    .OrderBy(feature => feature.Product)
                                    .Select(feature => feature.Product)
                                    .Distinct()
                                    .ToListAsync();
            }
        }

        /// <summary>
        /// Deletes the specified product.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        public async Task DeleteProductAsync(string product)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync(
                    nameof(Features_ByTitleProductAndGroup).Replace('_', '/'),
                    new IndexQuery() {Query = $"Product:\"{product}\"" },
                    new BulkOperationOptions() {AllowStale = true});
            }
        }

        /// <summary>
        /// Deletes a specified version of the specified product.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        /// <param name="version">The version of the product to delete.</param>
        public async Task DeleteProductAsync(string product, string version)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync(
                    nameof(Features_ByTitleProductAndGroup).Replace('_', '/'),
                    new IndexQuery() { Query = $"Product:\"{product}\"AND Version:\"{version}\"" },
                    new BulkOperationOptions() { AllowStale = true });
            }
        }

        /// <summary>
        /// Gets all available tags for the provided <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the available tags for.</param>
        /// <returns>Returns a range of tags for the provided <paramref name="productName">product</paramref>.</returns>
        public async Task<IEnumerable<string>> GetTagsAsync(string productName)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                return await session.Query<Features_ByProductAndBranch.TaggedFeature, Features_ByProductAndBranch>()
                                    .Where(feature => feature.Product.Equals(productName, StringComparison.CurrentCultureIgnoreCase))
                                    .OrderBy(feature => feature.Tag)
                                    .Select(feature => feature.Tag)
                                    .Distinct()
                                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets all available tags for the provided <paramref name="version"/> of <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the available tags for.</param>
        /// <param name="version">The version to get the available tags for.</param>
        /// <returns>Returns a range of tags for the provided <paramref name="version"/> of <paramref name="productName">product</paramref>.</returns>
        public async Task<IEnumerable<string>> GetTagsForVersionAsync(string productName, string version)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                return await session.Query<Features_ByProductAndBranch.TaggedFeature, Features_ByProductAndBranch>()
                                    .Where(feature => feature.Product.Equals(productName, StringComparison.CurrentCultureIgnoreCase) &&
                                                      feature.Version.Equals(version, StringComparison.CurrentCultureIgnoreCase))
                                    .OrderBy(feature => feature.Tag)
                                    .Select(feature => feature.Tag)
                                    .Distinct()
                                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the versions for the specified <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the available versions for.</param>
        /// <returns>A range of versions.</returns>
        public async Task<IEnumerable<string>> GetVersionsAsync(string productName)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var versions = await session.Query<DbFeature, Features_ByProductAndBranch>()
                                    .Where(feature => feature.Product.Equals(productName, StringComparison.CurrentCultureIgnoreCase))
                                    .Select(feature => feature.Version)
                                    .Distinct()
                                    .ToListAsync();

                return versions.OrderByDescending(version => version, new SemanticVersionComparer());
            }
        }
    }
}
