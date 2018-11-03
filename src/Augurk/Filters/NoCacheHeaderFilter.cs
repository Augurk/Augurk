/*
 Copyright 2014-2015, Mark Taling
 
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

using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;

namespace Augurk.Api.Filters
{
    /// <summary>
    /// A filter that ensures the response of the current web request will not be cached on the client.
    /// </summary>
    public class NoCacheHeaderFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Adds or alters the <see cref="CacheControlHeaderValue"/> on the response headers to ensure the repsonse will not be cached.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO: Figure out how to handle this with ASP.NET Core
            /*
            if (context.HttpContext.Response == null)
            {
                context.HttpContext.Response = new HttpResponseMessage();
            }

            if (context.Response.Headers.CacheControl == null)
            {
                context.Response.Headers.CacheControl = new CacheControlHeaderValue();
            }

            context.Response.Headers.CacheControl.NoCache = true;
            context.Response.Headers.CacheControl.NoStore = true;
            context.Response.Headers.CacheControl.MustRevalidate = true;

            context.Response.Headers.Add("Pragma", "no-cache");
            */
        }
    }
}

