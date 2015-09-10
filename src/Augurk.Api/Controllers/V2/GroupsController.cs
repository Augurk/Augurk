using Augurk.Api.Managers;
using Augurk.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available groups of related features within a product.
    /// </summary>
    [RoutePrefix("api/v2/products/{productName}/groups")]
    public class GroupsController : ApiController
    {
        private readonly FeatureManager _featureManager = new FeatureManager();

        /// <summary>
        /// Gets all the groups for the provided <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the groups for.</param>
        /// <returns>Returns a range of groups.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<Group>> GetGroupsAsync(string productName)
        {
            return await _featureManager.GetGroupedFeatureDescriptionsAsync(productName);
        }
    }
}
