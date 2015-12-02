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

        /// <summary>
        /// Deletes all versions of the provided group from the database.
        /// </summary>
        /// <param name="productName">Name of the product that the group belongs to.</param>
        /// <param name="groupName">Name of the group that should be deleted.</param>
        [Route("{groupName}")]
        [HttpDelete]
        public async Task DeleteGroupAsync(string productName, string groupName)
        {
            await _featureManager.DeleteFeaturesAsync(productName, groupName);
        }

        /// <summary>
        /// Deletes the provided version of the provided group from the database.
        /// </summary>
        /// <param name="productName">Name of the product that the group belongs to.</param>
        /// <param name="groupName">Name of the group that should be deleted.</param>
        /// <param name="version">The version of the group that should be deleted.</param>
        [Route("{groupName}/versions/{version}")]
        [HttpDelete]
        public async Task DeleteGroupAsync(string productName, string groupName, string version)
        {
            await _featureManager.DeleteFeaturesAsync(productName, groupName, version);
        }
    }
}
