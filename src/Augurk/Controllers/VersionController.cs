/*
 Copyright 2018, Augurk
 
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

using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Augurk.Api.Controllers
{
    /// <summary>
    /// ApiController for retrieving the currently installed Augurk version.
    /// </summary>
    [ApiVersionNeutral]
    [Route("api/version")]
    public class VersionController : Controller
    {
        /// <summary>
        /// Gets the version of this Augurk instance.
        /// </summary>
        /// <returns>The versionnumber.</returns>
        [HttpGet]
        public string GetVersion()
        {
            return GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}
