/*
 Copyright 2014, Mark Taling
 
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

using System.Net.Http.Headers;
using System.Web.Http.Filters;

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
        /// <param name="actionExecutedContext">The current context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response.Headers.CacheControl == null)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue();
            }

            actionExecutedContext.Response.Headers.CacheControl.NoCache = true;
            actionExecutedContext.Response.Headers.CacheControl.NoStore = true;
            actionExecutedContext.Response.Headers.CacheControl.MustRevalidate = true;

            actionExecutedContext.Response.Headers.Add("Pragma", "no-cache");
        }
    }
}

