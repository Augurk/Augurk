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

namespace Augurk.Entities.Test
{
    /// <summary>
    /// Represents the test result of a single feature.
    /// </summary>
    public class FeatureTestResult
    {
        /// <summary>
        /// Gets or sets the title of the feature.
        /// </summary>
        public string FeatureTitle { get; set; }

        /// <summary>
        /// Gets or sets the overal test result of the feature.
        /// </summary>
        public TestResult Result { get; set; }

        /// <summary>
        /// Gets or sets an enumerable collection of the test results of the scenario's that are part of the feature.s
        /// </summary>
        public IEnumerable<ScenarioTestResult> ScenarioTestResults { get; set; }
    }
}
