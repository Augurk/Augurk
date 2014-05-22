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
    /// Represents a rest result
    /// </summary>
    public enum TestResult
    {
        /// <summary>
        /// There is no testresult
        /// </summary>
        None = 0,

        /// <summary>
        /// The test has passed
        /// </summary>
        Passed = 1,

        /// <summary>
        /// The testresults are inconclusive
        /// </summary>
        Inconclusive = 2,

        /// <summary>
        /// The test has failed
        /// </summary>
        Failed = 3
    }
}
