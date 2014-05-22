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

using System.Collections.Generic;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents an example set
    /// </summary>
    public class ExampleSet : Table
    {
        /// <summary>
        /// Gets or sets the title of this example set.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of this example set.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the keyword for this example set.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the tags of this example set.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
