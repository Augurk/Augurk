using Augurk.Api.Managers;
using Augurk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetProducts()
        {
            return await _productsManager.GetProductsAsync();
        }
    }
}
