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
using System.Xml;
using System.Xml.Linq;
using Augurk.Entities;
using Augurk.Entities.Test;

namespace Augurk.MSBuild.TestResultParsers
{
    /// <summary>
    /// Provides methods to parse the results of an MS-Test run
    /// </summary>
    public class MsTestResultParser : ITestResultParser
    {
        private static readonly XNamespace NS = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        /// <summary>
        /// Parses the provided file at the provided <paramref name="testResultFilePath"/>.
        /// </summary>
        /// <param name="testResultFilePath">The path at witch the file containing the testresults can be found.</param>
        /// <returns>A collection of <see cref="FeatureTestResult"/> instances containing the testresults for all executed scenarios.</returns>
        public IEnumerable<FeatureTestResult> Parse(string testResultFilePath)
        {
            XElement root = XElement.Load(testResultFilePath);

            var features = from unitTest in root.Element(NS + "TestDefinitions").Elements(NS + "UnitTest")
                           let properties = ParseProperties(unitTest)
                           where properties.ContainsKey("FeatureTitle")
                           group new {unitTest, properties} by properties["FeatureTitle"]
                           into feature
                           let scenarios =
                                (from scenario in feature
                                 let excutionId = scenario.unitTest.Element(NS + "Execution").Attribute("id").Value
                                 let resultElement = GetResult(excutionId, root)
                                 select new ScenarioTestResult()
                                     {
                                         ScenarioTitle = scenario.unitTest.Element(NS + "Description").Value,
                                         VariantName = scenario.properties.ContainsKey("VariantName") ? scenario.properties["VariantName"] : String.Empty,
                                         Result = resultElement == null ? TestResult.None : (TestResult)Enum.Parse(typeof(TestResult), resultElement.Attribute("outcome").Value),
                                         TestExecutionDate = resultElement == null ? (DateTime?)null : XmlConvert.ToDateTime(resultElement.Attribute("endTime").Value)
                                     }).ToList()
                           select new FeatureTestResult()
                               {
                                   FeatureTitle = feature.Key,
                                   ScenarioTestResults = scenarios,
                                   Result = scenarios.Max(scenario => scenario.Result),
                                   TestExecutionDate = scenarios.Where(scenario => scenario.TestExecutionDate.HasValue).Max(scenario => scenario.TestExecutionDate.Value)
                               };

            // Force execution before returning
            return features.ToList();
        }

        private IDictionary<string, string> ParseProperties(XElement unitTestElement)
        {
            XElement propertiesElement = unitTestElement.Element(NS + "Properties");

            if (propertiesElement == null)
            {
                return new Dictionary<string, string>();
            }

            return propertiesElement.Elements(NS + "Property")
                                    .ToDictionary(property => property.Element(NS + "Key").Value,
                                                  property => property.Element(NS + "Value").Value);
        }

        private XElement GetResult(string executionId, XElement root)
        {
            XElement resultElement = root.Element(NS + "Results")
                                            .Elements(NS + "UnitTestResult")
                                            .SingleOrDefault(unitTestResultElement => String.Equals(unitTestResultElement.Attribute("executionId").Value, executionId, StringComparison.InvariantCultureIgnoreCase));

            return resultElement;
        }
    }
}
