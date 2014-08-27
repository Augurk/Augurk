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
using Augurk.Entities.Test;

namespace Augurk.MSBuild.TestResultParsers
{
    /// <summary>
    /// Defines methods to parse the results of an MS-Test run
    /// </summary>
    public interface ITestResultParser
    {

        /// <summary>
        /// Parses the provided file at the provided <paramref name="testResultFilePath"/>.
        /// </summary>
        /// <param name="testResultFilePath">The path at witch the file containing the testresults can be found.</param>
        /// <returns>An collection of <see cref="FeatureTestResult"/> instances containing the testresults for all executed scenarios.</returns>
        IEnumerable<FeatureTestResult> Parse(string testResultFilePath);
    }
}