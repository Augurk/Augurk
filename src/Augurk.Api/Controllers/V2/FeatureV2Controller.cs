using Augurk.Api.Managers;
using Augurk.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<FeatureDescription>> GetAsync(string productName, string groupName)
        {
            return await _featureManager.GetFeatureDescriptionsByProductAndGroupAsync(productName, groupName);
        }
    }
}
