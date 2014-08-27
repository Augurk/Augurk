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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Augurk.Entities;
using Augurk.Entities.Test;
using Augurk.MSBuild.TestResultParsers;

namespace Augurk.MSBuild.UnitTest.TestResultParsers
{
    /// <summary>
    /// Contains all tests for the <see cref="MsTestResultParser"/> class.
    /// </summary>
    [TestClass]
    public class MsTestResultParserTest
    {
        [DeploymentItem("TestData\\MsTestOutput.trx")]
        [TestMethod]
        public void Parse()
        {
            // Arrange
            ITestResultParser parser = new MsTestResultParser();

            // Act
            IEnumerable<FeatureTestResult> results = parser.Parse("MsTestOutput.trx");

            // Assert
            Assert.IsNotNull(results);
            
            List<FeatureTestResult> resultList = results.OrderBy(result => result.FeatureTitle).ToList();
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(TestResult.Failed, resultList[0].Result);
            Assert.AreEqual(TestResult.Passed, resultList[1].Result);
            Assert.AreEqual(new DateTime(2014, 05, 09, 5, 58, 38).ToString(), resultList[0].TestExecutionDate.ToString());
            Assert.AreEqual(new DateTime(2014, 05, 09, 5, 58, 39).ToString(), resultList[1].TestExecutionDate.ToString());

            List<ScenarioTestResult> variantScenarioList = resultList[0].ScenarioTestResults.OrderBy(result => result.VariantName).ToList();
            Assert.AreEqual(2, variantScenarioList.Count);
            Assert.AreEqual("Variant 0", variantScenarioList[0].VariantName);
            Assert.AreEqual(TestResult.Failed, variantScenarioList[0].Result);
            Assert.AreEqual("Variant 1", variantScenarioList[1].VariantName);
            Assert.AreEqual(TestResult.Passed, variantScenarioList[1].Result);
        }
    }
}
