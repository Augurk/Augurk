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

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a single step within a scenario
    /// </summary>
    public class Step
    {
        /// <summary>
        /// Gets or sets the block keyword for this step.
        /// </summary>
        public BlockKeyword BlockKeyword { get; set; }

        /// <summary>
        /// Gets or sets the keyword for this step.
        /// </summary>
        public StepKeyword StepKeyword { get; set; }

        /// <summary>
        /// Gets or sets the localized keyword for this step.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the content of this step.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the table argument for this step.
        /// </summary>
        public Table TableArgument { get; set; }
    }
}