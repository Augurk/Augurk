/*
 Copyright 2017, Augurk
 
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
using Augurk.Entities;

namespace Augurk.Api
{
    /// <summary>
    /// Provides default values for items which should otherwise be configured in the database.
    /// </summary>
    internal static class Defaults
    {
        /// <summary>
        /// Gets the default customization.
        /// </summary>
        /// <remarks>
        /// Each call will result a new instance being returned.
        /// </remarks>
        public static Customization Customization =>
            new Customization
            {
                InstanceName = "Augurk",
            };

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        /// <remarks>
        /// Each call will result a new instance being returned.
        /// </remarks>
        public static Configuration Configuration =>
            new Configuration
            {
                ExpirationEnabled = false,
                ExpirationDays = 30,
                ExpirationRegex = "[0-9.]+-.*",
            };
    }
}
