/*
 Copyright 2014-2019, Augurk
 
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
            return await _productManager.GetProductsAsync();
        }
    }
}