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

using System;

namespace Augurk.Entities.Test
{
    /// <summary>
    /// Represents the test result of a single scenario (or variant of a scenario).
    /// </summary>
    public class ScenarioTestResult
    {
        /// <summary>
        /// Gets or sets the title of the scenario.
        /// </summary>
        public string ScenarioTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the variant that was tested.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// Gets or sets the result of the test.
        /// </summary>
        public TestResult Result { get; set; }

        /// <summary>
        /// Gets or sets the date and time on which this test has been executed.
        /// </summary>
        public DateTime? TestExecutionDate { get; set; }
    }
}
