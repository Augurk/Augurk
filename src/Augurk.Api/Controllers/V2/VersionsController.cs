using Augurk.Api.Managers;
using Augurk.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available versions within a product.
    /// </summary>
    [RoutePrefix("api/v2/products/{productName}/versions")]
    public class VersionsController : ApiController
    {
        private readonly ProductManager _productsManager = new ProductManager();
        private readonly FeatureManager _featureManager = new FeatureManager();

        /// <summary>
        /// Gets all available versions of the given <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the versions for.</param>
        /// <returns>A range of versions.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetVersionsAsync(string productName)
        {
            return await _productsManager.GetVersionsAsync(productName);
        }

        /// <summary>
        /// Gets the tags placed on features contained within a <paramref name="version"/> of the specified <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the tags for.</param>
        /// <param name="version">Version of the product to get the tags for.</param>
        /// <returns>A range of tags.</returns>
        [Route("{version}/tags")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetTagsAsync(string productName, string version)
        {
            return await _productsManager.GetTagsForVersionAsync(productName, version);
        }

        /// <summary>
        /// Gets the features that contain the given <paramref name="tag" /> within a <paramref name="version"/> of the specified <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the features containing the given tag.</param>
        /// <param name="version">Version of the product to get the features containing the given tag.</param>
        /// <param name="tag">The tag to search for in the features.</param>
        /// <returns>A range of features.</returns>
        [Route("{version}/tags/{tag}/features")]
        [HttpGet]
        public async Task<IEnumerable<FeatureDescription>> GetFeaturesByProductVersionAndTagAsync(string productName, string version, string tag)
        {
            return await _featureManager.GetFeatureDescriptionsByProductVersionAndTagAsync(productName, version, tag);
        }

        /// <summary>
        /// Gets the groups within the given <paramref name="version"/> of the specified <paramref name="productName">product</paramref>.
        /// </summary>
        /// <param name="productName">Name of the product to get the tags for.</param>
        /// <param name="version">Version of the product to get the tags for.</param>
        /// <returns>A range of groups.</returns>
        [Route("{version}/groups")]
        [HttpGet]
        public async Task<IEnumerable<Group>> GetGroupsAsync(string productName, string version)
        {
            return await _featureManager.GetGroupedFeatureDescriptionsAsync(productName, version);
        }
    }
}
