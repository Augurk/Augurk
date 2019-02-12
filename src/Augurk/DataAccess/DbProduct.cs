/*
 Copyright 2017-2019, Augurk
  
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

namespace Augurk.Api
{
    /// <summary>
    /// An implementation of a product designed specifically for storage in the database.
    /// </summary>
    public class DbProduct
    {
        /// <summary>
        /// Gets or sets the name of this product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this product in markdown syntax.
        /// </summary>
        public string DescriptionMarkdown { get; set; }
    }
}
