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

namespace Augurk.Entities
{
    /// <summary>
    /// Contains the Augurk configuration.
    /// </summary>
    public class Configuration
    {
        #region Expiration
        /// <summary>
        /// Indicates whether the expiration of features is enabled.
        /// </summary>
        public bool ExpirationEnabled;

        /// <summary>
        /// Defines the number of days after which an features should expire.
        /// </summary>
        public int ExpirationDays;

        /// <summary>
        /// Defines the regular expression which a feature version must match to expire.
        /// </summary>
        public string ExpirationRegex;
        #endregion
    }
}
