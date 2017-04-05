using System;
using Augurk.Api.Managers;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available products.
    /// </summary>
    [RoutePrefix("api/v2/products")]
    public class ProductsController : ApiController
    {
        private readonly ProductManager _productsManager = new ProductManager();

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
