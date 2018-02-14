/*
 Copyright 2018, Augurk
 
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

namespace Augurk.Entities.Analysis
{
    public class AnalysisReport
    {
        /// <summary>
        /// The name of the analyzed project.
        /// </summary>
        /// <remarks>
        /// Augurk will use this name to display where the code for an automation was found.
        /// </remarks>
        public string AnalyzedProject { get; set; }

        /// <summary>
        /// The version of the code that was analyzed.
        /// </summary>
        /// <remarks>
        /// This field will be overwritten by the value provided during upload.
        /// </remarks>
        public string Version { get; set; }

        /// <summary>
        /// Indicates when this analysis was run.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The <see cref="Invocation">invocations</see> that were discovered during analysis.
        /// </summary>
        public Invocation[] RootInvocations { get; set; }
    }
}
