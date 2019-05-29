/*
 Copyright 2019, Augurk

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
using Augurk.Api;
using Augurk.Api.Managers;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit;
using NSubstitute;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Shouldly;

namespace Augurk.Test.Managers
{
    /// <summary>
    /// Contains unit tests for the <see cref="DependencyManager" /> class.
    /// </summary>
    public class DependencyManagerTests
    {
        private readonly IFeatureManager _featureManager = Substitute.For<IFeatureManager>();
        private readonly IAnalysisReportManager _analysisReportManager = Substitute.For<IAnalysisReportManager>();
        private readonly ILogger<DependencyManager> _logger = Substitute.For<ILogger<DependencyManager>>();

        /// <summary>
        /// Verifies whether feature graphs can be discovered when two different features are available.
        /// </summary>
        [Fact]
        public async Task VerifyFeatureGraphsCanBeDiscovered()
        {
            // Arrange
            var callingFeature = CreateDbFeature("CallingFeature", "SomeClass.Foo()");
            var calledFeature = CreateDbFeature("CalledFeature", "SomeOtherClass.Bar()");
            _featureManager.GetAllDbFeatures().Returns(new[] { callingFeature, calledFeature });
            _analysisReportManager.GetAllDbInvocations().Returns(new DbInvocation[]
            {
                new DbInvocation()
                {
                    Signature = "SomeClass.Foo()",
                    InvokedSignatures = new []
                    {
                        "SomeClass.Foo(System.String)",
                        "SomeOtherClass.Bar()"
                    }
                },
                new DbInvocation()
                {
                    Signature = "SomeOtherClass.Bar()",
                    InvokedSignatures = new []
                    {
                        "SomeOtherClass.JuiceBar()"
                    }
                }
            });

            // Act
            var target = new DependencyManager(_featureManager, _analysisReportManager, _logger);
            var graphs = (await target.GetTopLevelFeatureGraphsAsync()).ToList();

            // Assert
            graphs.Count.ShouldBe(1);
            graphs[0].FeatureName.ShouldBe("CallingFeature");
            graphs[0].DependsOn.Count.ShouldBe(1);
            graphs[0].DependsOn[0].FeatureName.ShouldBe("CalledFeature");
        }

        /// <summary>
        /// Verifies whether feature graphs can be discovered even when there are features without signatures.
        /// </summary>
        [Fact]
        public async Task VerifyFeatureGraphsDiscoveryIsResistantToFeaturesWithoutSignatures()
        {
            // Arrange
            var callingFeature = CreateDbFeature("CallingFeature", "SomeClass.Foo()");
            var calledFeature = CreateDbFeature("CalledFeature", "SomeOtherClass.Bar()");
            var unlinkedFeature = CreateDbFeature("UnlinkedFeature");
            _featureManager.GetAllDbFeatures().Returns(new[] { callingFeature, calledFeature, unlinkedFeature });
            _analysisReportManager.GetAllDbInvocations().Returns(new DbInvocation[]
            {
                new DbInvocation()
                {
                    Signature = "SomeClass.Foo()",
                    InvokedSignatures = new []
                    {
                        "SomeClass.Foo(System.String)",
                        "SomeOtherClass.Bar()"
                    }
                },
                new DbInvocation()
                {
                    Signature = "SomeOtherClass.Bar()",
                    InvokedSignatures = new []
                    {
                        "SomeOtherClass.JuiceBar()"
                    }
                }
            });

            // Act
            var target = new DependencyManager(_featureManager, _analysisReportManager, _logger);
            var graphs = (await target.GetTopLevelFeatureGraphsAsync()).ToList();

            // Assert
            graphs.Count.ShouldBe(2);
            graphs.Any(f => f.FeatureName == "CallingFeature").ShouldBeTrue();
            graphs.Any(f => f.FeatureName == "UnlinkedFeature").ShouldBeTrue();
        }

        /// <summary>
        /// Verifies whether a lower level feature graph can be retrieved directly.
        /// </summary>
        [Fact]
        public async Task VerifyAMidTreeFeatureGraphCanBeRetrieved()
        {
            // Arrange
            var callingFeature = CreateDbFeature("CallingFeature", "SomeClass.Foo()");
            var calledFeature = CreateDbFeature("CalledFeature", "SomeOtherClass.Bar()");
            var anotherCalledFeature = CreateDbFeature("AnotherCalledFeature", "SomeOtherClass.JuiceBar()");
            _featureManager.GetAllDbFeatures().Returns(new[] { callingFeature, calledFeature, anotherCalledFeature });
            _analysisReportManager.GetAllDbInvocations().Returns(new DbInvocation[]
            {
                new DbInvocation()
                {
                    Signature = "SomeClass.Foo()",
                    InvokedSignatures = new []
                    {
                        "SomeClass.Foo(System.String)",
                        "SomeOtherClass.Bar()"
                    }
                },
                new DbInvocation()
                {
                    Signature = "SomeOtherClass.Bar()",
                    InvokedSignatures = new []
                    {
                        "SomeOtherClass.JuiceBar()"
                    }
                }
            });

            // Act
            var target = new DependencyManager(_featureManager, _analysisReportManager, _logger);
            var graph = await target.GetFeatureGraphAsync("TestProduct", "CalledFeature", "0.0.0");

            // Assert
            graph.FeatureName.ShouldBe("CalledFeature");
            graph.DependsOn.Count.ShouldBe(1);
            graph.DependsOn[0].FeatureName.ShouldBe("AnotherCalledFeature");
            graph.Dependants.Count.ShouldBe(1);
            graph.Dependants[0].FeatureName.ShouldBe("CallingFeature");
        }

        /// <summary>
        /// Verifies whether a feature graph can be retrieved directly, even when it's code has some recursion going on.
        /// </summary>
        [Fact]
        public async Task VerifyAFeatureGraphCanBeRetrievedWhenItsCodeHasRecursion()
        {
            // Arrange
            var callingFeature = CreateDbFeature("CallingFeature", "SomeClass.Foo()");
            var calledFeature = CreateDbFeature("CalledFeature", "SomeOtherClass.Bar()");
            var anotherCalledFeature = CreateDbFeature("AnotherCalledFeature", "SomeOtherClass.JuiceBar()");
            _featureManager.GetAllDbFeatures().Returns(new[] { callingFeature, calledFeature, anotherCalledFeature });
            _analysisReportManager.GetAllDbInvocations().Returns(new DbInvocation[]
            {
                new DbInvocation()
                {
                    Signature = "SomeClass.Foo()",
                    InvokedSignatures = new []
                    {
                        "SomeClass.Foo(System.String)",
                        "SomeOtherClass.Bar()"
                    }
                },
                new DbInvocation()
                {
                    Signature = "SomeOtherClass.Bar()",
                    InvokedSignatures = new []
                    {
                        "SomeOtherClass.JuiceBar()"
                    }
                },
                new DbInvocation()
                {
                    Signature = "SomeOtherClass.JuiceBar()",
                    InvokedSignatures = new []
                    {
                        "SomeOtherClass.Bar()"
                    }
                }
            });

            // Act
            var target = new DependencyManager(_featureManager, _analysisReportManager, _logger);
            var graph = await target.GetFeatureGraphAsync("TestProduct", "CalledFeature", "0.0.0");

            // Assert
            graph.FeatureName.ShouldBe("CalledFeature");
            graph.DependsOn.Count.ShouldBe(1);
            graph.DependsOn[0].FeatureName.ShouldBe("AnotherCalledFeature");
            graph.Dependants.Count.ShouldBe(2);
            graph.Dependants[0].FeatureName.ShouldBe("CallingFeature");
            graph.Dependants[1].FeatureName.ShouldBe("AnotherCalledFeature");
        }

        private static DbFeature CreateDbFeature(string featureName, params string[] directInvocationSignatures)
        {
            var result = new DbFeature()
            {
                Product = "TestProduct",
                Group = "TestGroup",
                Title = featureName,
                Version = "0.0.0",
            };

            if (directInvocationSignatures != null && directInvocationSignatures.Length > 0)
            {
                result.DirectInvocationSignatures = new List<string>(directInvocationSignatures);
            }

            return result;
        }
    }
}
