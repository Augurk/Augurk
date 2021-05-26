/*
 Copyright 2021, Augurk

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
    /// Represents a rest result
    /// </summary>
    public class PostResult
    {
        /// <summary>
        /// The Uniform Resource Locator at which the API provides the posted content.
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// The Uniform Resource Locator at which the Augurk UI provides the posted content.
        /// </summary>
        public string WebUrl { get; set; }
    }
}
