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
    /// Represents the Gherkin keywords
    /// </summary>
    public enum StepKeyword
    {
        /// <summary>
        /// No keyword.
        /// </summary>
        None,
        /// <summary>
        /// The Given keyword.
        /// </summary>
        Given,
        /// <summary>
        /// The Then keyword.
        /// </summary>
        Then,
        /// <summary>
        /// The When keyword.
        /// </summary>
        When,
        /// <summary>
        /// The And keyword.
        /// Usage of this keyword masks the actual meaning, check the <see cref="BlockKeyword"/> for that.
        /// </summary>
        And
    }
}