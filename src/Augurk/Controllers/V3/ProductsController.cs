/*
 Copyright 2018, Augurk

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


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Api.Managers;
using Augurk.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Augurk.Api.Controllers.V3
{
    [ApiVersion("3.0")]
    [Route("api/v{apiVersion:apiVersion}/products")]
    public class ProductsController : Controller
    {
        private readonly ProductManager _productManager;
        private readonly FeatureManager _featureManager;

        public ProductsController(ProductManager productManager, FeatureManager featureManager)
        {
            _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetProducts()
        {
            var result = new List<Product>();
            var products = await _productManager.GetProductsAsync();
            foreach (var product in products)
            {
                var productDescription = await _productManager.GetProductDescriptionAsync(product);
                result.Add(new Product
                {
                    Name = product,
                    Description = productDescription
                });
            }

            return result;
        }

        [HttpGet]
        [Route("{productName}")]
        public async Task<ActionResult<ProductDetails>> GetProductDetails(string productName)
        {
            var productDescription = await _productManager.GetProductDescriptionAsync(productName);
            if (productDescription == null)
            {
                return NotFound();
            }

            var groups = await _featureManager.GetGroupedFeatureDescriptionsAsync(productName);

            return new ProductDetails
            {
                Name = productName,
                Description = productDescription,
                Groups = groups
            };
        }
    }
}