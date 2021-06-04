// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
