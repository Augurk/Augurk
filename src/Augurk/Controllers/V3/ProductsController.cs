// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Api.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Augurk.Entities;
using System.Linq;

namespace Augurk.Api.Controllers.V3
{
    /// <summary>
    /// ApiController for retrieving the available products.
    /// </summary>
    [ApiVersion("3.0")]
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
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return (await _productsManager.GetProductsAsync()).ToList();
        }
    }
}
