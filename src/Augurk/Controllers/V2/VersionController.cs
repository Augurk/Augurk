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
            var assemblyVersion = GetType().Assembly.GetName().Version;

            return Ok(new
            {
                ProductVersion = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}",
                InformationalVersion = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
            });
        }
    }
}