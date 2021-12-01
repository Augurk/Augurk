// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    public interface IProductManager
    {

        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>Returns a collection containing all products.</returns>
        Task<IEnumerable<Product>> GetProductsAsync();

        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>Returns a range of productName names.</returns>
        Task<IEnumerable<string>> GetProductTitlesAsync();

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
