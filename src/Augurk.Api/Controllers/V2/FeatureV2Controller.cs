using Augurk.Api.Managers;
using Augurk.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Augurk.Api.Controllers.V2
{
    /// <summary>
    /// ApiController for retrieving the available features.
    /// </summary>
    [RoutePrefix("api/v2/products/{productName}/groups/{groupName}/features")]
    public class FeatureV2Controller : ApiController
    {
        private readonly FeatureManager _featureManager = new FeatureManager();

        /// <summary>
        /// Gets the available features.
        /// </summary>
        /// <param name="productName">Name of the product to which the feature belongs.</param>
        /// <param name="groupName">Name of the group to which the feature belongs.</param>
        /// <returns>Returns a range of <see cref="FeatureDescription"/> instance describing the features.</returns>
        [Route("")]
        [HttpGet]
        public async Task<IEnumerable<FeatureDescription>> GetFeaturesForProductAndGroupAsync(string productName, string groupName)
        {
            return await _featureManager.GetFeatureDescriptionsByProductAndGroupAsync(productName, groupName);
        }

        /// <summary>
        /// Gets a specific feature.
        /// </summary>
        /// <param name="productName">Name of the product to which the feature belongs.</param>
        /// <param name="groupName">Name of the group to which the feature belongs.</param>
        /// <param name="featureTitle">Title of the feature.</param>
        /// <returns>Returns a <see cref="DisplayableFeature"/>.</returns>
        [Route("{featureTitle}")]
        [HttpGet]
        public async Task<DisplayableFeature> GetFeatureAsync(string productName, string groupName, string featureTitle)
        {
            return await _featureManager.GetFeatureAsync(productName, groupName, featureTitle, "1.0.0");
        }

        /// <summary>
        /// Saves a new feature into the database.
        /// </summary>
        /// <param name="feature">A <see cref="Feature"/> instance to save.</param>
        /// <param name="productName">Name of the product that the feature belongs to.</param>
        /// <param name="groupName">Name of the group that the feature belongs to.</param>
        /// <param name="title">Title of the feature.</param>
        /// <param name="version">Version of feature.</param>
        /// <returns>Returns a reponse message indicating whether saving the feature succeeded.</returns>
        [Route("{title}/versions/{version}")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync(Feature feature, string productName, string groupName, string title, string version)
        {
            if (!feature.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The title provided by the POST data and the title in uri do not match!");
            }

            var response = Request.CreateResponse(HttpStatusCode.Created);

            try
            {
                await _featureManager.InsertOrUpdateFeatureAsync(feature, productName, groupName, version);
            }
            catch (Exception exception)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }

            return response;
        }
    }
}
