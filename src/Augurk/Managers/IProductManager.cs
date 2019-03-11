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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Augurk.Api.Managers
{
    public interface IProductManager
    {
        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>Returns a range of productName names.</returns>
        Task<IEnumerable<string>> GetProductsAsync();

        /// <summary>
        /// Gets the description of the provided product.
        /// </summary>
        /// <param name="productName">The name of the product for which the description should be retrieved.</param>
        /// <returns>The description of the requested product; or, null.</returns>
        Task<string> GetProductDescriptionAsync(string productName);


        /// <summary>
        /// Inserts or updates the provided description for the product with the provided name.
        /// </summary>
        /// <param name="productName">The name of the product for which the description should be persisted.</param>
        /// <param name="descriptionMarkdown">The description that should be persisted.</param>
        Task InsertOrUpdateProductDescriptionAsync(string productName, string descriptionMarkdown);

        /// <summary>
        /// Deletes the specified productName.
        /// </summary>
        /// <param name="productName">The name of the product to delete.</param>
        Task DeleteProductAsync(string productName);

        /// <summary>
        /// Deletes a specified version of the specified productName.
        /// </summary>
        /// <param name="productName">The productName to delete.</param>
        /// <param name="version">The version of the productName to delete.</param>
        Task DeleteProductAsync(string productName, string version);

        /// <summary>
        /// Gets all available tags for the provided <paramref name="productName">productName</paramref>.
        /// </summary>
        /// <param name="productName">Name of the productName to get the available tags for.</param>
        /// <returns>Returns a range of tags for the provided <paramref name="productName">productName</paramref>.</returns>
        Task<IEnumerable<string>> GetTagsAsync(string productName);
    }
}