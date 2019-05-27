using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Augurk.Api.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/version")]
    public class VersionController : Controller
    {
        /// <summary>
        /// Gets the version of this Augurk instance.
        /// </summary>
        /// <returns>The versionnumber.</returns>
        [HttpGet]
        public ActionResult GetVersion()
        {
            return Ok(new
            {
                ProductVersion = GetType().Assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "Development",
                InformationalVersion = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            });
        }
    }
}