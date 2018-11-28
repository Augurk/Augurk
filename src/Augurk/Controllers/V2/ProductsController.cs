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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available products.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/products")]
    public class ProductsController : Controller
    {
        private readonly IProductManager _productsManager;

        public ProductsController(IProductManager productManager)
        {
            _productsManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
        }

        /// <summary>
        /// Gets all available products.
        /// </summary>
        /// <returns>A range of product names.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetProductsAsync()
        {
            return await _productsManager.GetProductsAsync();
        }

        /// <summary>
        /// Gets the requested product description.
        /// </summary>
        /// <returns>The description of the requested product.</returns>
        [Route("{productName}/description")]
        [HttpGet]
        public async Task<string> GetProductDescriptionAsync(string productName)
        {
            return await _productsManager.GetProductDescriptionAsync(productName);
        }

        /// <summary>
        /// Puts the provided product description.
        /// </summary>
        [Route("{productName}/description")]
        [HttpPut]
        public async Task PutProductDescriptionAsync(string productName, [FromBody]string descriptionMarkdown)
        {
            await _productsManager.InsertOrUpdateProductDescriptionAsync(productName, descriptionMarkdown);
        }

        /// <summary>
        /// Deletes the provided product.
        /// </summary>
        [Route("{productName}")]
        [HttpDelete]
        public async Task DeleteProductAsync(string productName)
        {
            await _productsManager.DeleteProductAsync(productName);
        }

        /// <summary>
        /// Deletes the provided version of the provided product.
        /// </summary>
        [Route("{productName}/versions/{version}")]
        [HttpDelete]
        public async Task DeleteProductAsync(string productName, string version)
        {
            await _productsManager.DeleteProductAsync(productName, version);
        }

        /// <summary>
        /// Gets the tags placed on features contained within the specified <paramref name="productName">product</paramref>.
        /// </summary>
        /// <returns>Returns a range of tags.</returns>
        [Route("{productName}/tags")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetTagsAsync(string productName)
        {
            return await _productsManager.GetTagsAsync(productName);
        }
    }
}
