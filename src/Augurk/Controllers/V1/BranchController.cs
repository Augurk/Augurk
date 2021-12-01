// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Api.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Augurk.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/branches")]
    [Route("api/v{apiVersion:apiVersion}/branches")]
    public class BranchController : Controller
    {
        private readonly IProductManager _productManager;

        public BranchController(IProductManager productManager)
        {
            _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            // NOTE: Using the ProductManager for backwards compatability
            return await _productManager.GetProductTitlesAsync();
        }
    }
}
