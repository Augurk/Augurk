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
using System;
using System.Threading.Tasks;
using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Analysis;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using Raven.Client;
using Augurk.Api.Indeces.Analysis;
using System.Linq;
using Augurk.Api;

namespace Augurk.Test.Managers
{
    /// <summary>
    /// Contains tests for the <see cref="AnalysisReportManager" /> class.
    /// </summary>
    public class AnalysisReportManagerTests : RavenTestBase
    {
        private readonly IConfigurationManager configurationManager = Substitute.For<IConfigurationManager>();
        private readonly ILogger<AnalysisReportManager> logger = Substitute.For<ILogger<AnalysisReportManager>>();

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> can store a <see cref="AnalysisReport" /> instance.
        /// </summary>
        [Fact]
        public async Task StoresAnalysisReport()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            var expectedReport = new AnalysisReport
            {
                AnalyzedProject = "MyProject",
                Timestamp = DateTime.Now,
                Version = "0.0.0"
            };

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            await sut.InsertOrUpdateAnalysisReportAsync("MyProduct", "0.0.0", expectedReport);

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualReport = await session.LoadAsync<AnalysisReport>("MyProduct/0.0.0/MyProject");
                actualReport.ShouldNotBeNull();
                actualReport.Timestamp.ShouldBe(expectedReport.Timestamp);
                actualReport.Version.ShouldBe("0.0.0");

                var actualMetadata = session.Advanced.GetMetadataFor(actualReport);
                actualMetadata["Product"].ShouldBe("MyProduct");
            }
        }

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> sets an expiration on a <see cref="AnalysisReport" />
        /// if that is configured.
        /// </summary>
        [Fact]
        public async Task SetsExpirationOnAnalysisReportsIfConfigured()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            configurationManager.GetOrCreateConfigurationAsync().Returns(new Configuration
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"\d\.\d\.\d"
            });

            var expectedReport = new AnalysisReport
            {
                AnalyzedProject = "MyProject",
            };

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            await sut.InsertOrUpdateAnalysisReportAsync("MyProduct", "0.0.0", expectedReport);

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualReport = await session.LoadAsync<AnalysisReport>("MyProduct/0.0.0/MyProject");
                actualReport.ShouldNotBeNull();

                var actualMetadata = session.Advanced.GetMetadataFor(actualReport);
                actualMetadata[Constants.Documents.Metadata.Expires].ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> is able to retrieve previously stored 
        /// <see cref="AnalysisReport" /> instances.
        /// </summary>
        [Fact]
        public async Task GetsAnalysisReports()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            await documentStoreProvider.Store.ExecuteIndexAsync(new AnalysisReports_ByProductAndVersion());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var expectedReport1 = new AnalysisReport { Version = "0.0.0", AnalyzedProject = "Project1" };
                var expectedReport2 = new AnalysisReport { Version = "0.0.0", AnalyzedProject = "Project2" };
                var expectedReport3 = new AnalysisReport { Version = "0.0.0", AnalyzedProject = "Project3" };

                await session.StoreAsync(expectedReport1);
                await session.StoreAsync(expectedReport2);
                await session.StoreAsync(expectedReport3);

                session.Advanced.GetMetadataFor(expectedReport1)["Product"] = "Product1";
                session.Advanced.GetMetadataFor(expectedReport2)["Product"] = "Product1";
                session.Advanced.GetMetadataFor(expectedReport3)["Product"] = "Product2";

                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            var reports = sut.GetAnalysisReportsByProductAndVersionAsync("Product1", "0.0.0");

            // Assert
            reports.ShouldNotBeNull();
            reports.Count().ShouldBe(2);

            var actualReport1 = reports.First();
            actualReport1.Version.ShouldBe("0.0.0");
            actualReport1.AnalyzedProject.ShouldBe("Project1");

            var actualReport2 = reports.Last();
            actualReport2.Version.ShouldBe("0.0.0");
            actualReport2.AnalyzedProject.ShouldBe("Project2");
        }

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> class can retrieve all <see cref="DbInvocation" /> instances
        /// from the database.
        /// </summary>
        [Fact]
        public async Task GetsAllDbInvocations()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(new DbInvocation { Signature = "Foo()" });
                await session.StoreAsync(new DbInvocation { Signature = "Bar()" });
                await session.SaveChangesAsync();
            }

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetAllDbInvocations();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);

            var invocation1 = result.FirstOrDefault();
            invocation1.ShouldNotBeNull();
            invocation1.Signature.ShouldBe("Foo()");

            var invocation2 = result.LastOrDefault();
            invocation2.ShouldNotBeNull();
            invocation2.Signature.ShouldBe("Bar()");
        }

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> can properly persist <see cref="DbInvocation" />
        /// instances to the database.
        /// </summary>
        [Fact]
        public async Task PersistsDbInvocations()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            var expectedInvocation1 = new DbInvocation { Signature = "Foo()" };
            var expectedInvocation2 = new DbInvocation { Signature = "Bar()" };

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            await sut.PersistDbInvocationsAsync("Product1", "0.0.0", new[] { expectedInvocation1, expectedInvocation2 });

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualInvocation1 = await session.LoadAsync<DbInvocation>("Product1/0.0.0/Foo()");
                actualInvocation1.ShouldNotBeNull();
                actualInvocation1.Signature.ShouldBe("Foo()");

                var actualInvocation1Metadata = session.Advanced.GetMetadataFor(actualInvocation1);
                actualInvocation1Metadata["Product"].ShouldBe("Product1");
                actualInvocation1Metadata["Version"].ShouldBe("0.0.0");

                var actualInvocation2 = await session.LoadAsync<DbInvocation>("Product1/0.0.0/Bar()");
                actualInvocation2.ShouldNotBeNull();
                actualInvocation2.Signature.ShouldBe("Bar()");

                var actualInvocation2Metadata = session.Advanced.GetMetadataFor(actualInvocation2);
                actualInvocation1Metadata["Product"].ShouldBe("Product1");
                actualInvocation1Metadata["Version"].ShouldBe("0.0.0");
            }
        }

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> class sets an expiration on <see cref="DbInvocation" />
        /// instances beings aved to the database if expiration is configured.
        /// </summary>
        [Fact]
        public async Task SetsExpirationOnDbInvocationsIfConfigured()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            var expectedInvocation = new DbInvocation { Signature = "Foo()" };
            configurationManager.GetOrCreateConfigurationAsync().Returns(new Configuration
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"\d\.\d\.\d"
            });

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            await sut.PersistDbInvocationsAsync("Product1", "0.0.0", new[] { expectedInvocation });

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualInvocation = await session.LoadAsync<DbInvocation>("Product1/0.0.0/Foo()");
                actualInvocation.ShouldNotBeNull();

                var actualInvocationMetadata = session.Advanced.GetMetadataFor(actualInvocation);
                actualInvocationMetadata[Constants.Documents.Metadata.Expires].ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="AnalysisReportManager" /> deletes <see cref="DbInvocation" /> instances
        /// from the database.
        /// </summary>
        [Fact]
        public async Task DeletesDbInvocations()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            await documentStoreProvider.Store.ExecuteIndexAsync(new Invocation_ByProductAndVersion());
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var invocation1 = new DbInvocation { Signature = "Foo()" };
                await session.StoreAsync(invocation1, $"Product1/0.0.0/{invocation1.Signature}");

                var invocation1Metadata = session.Advanced.GetMetadataFor(invocation1);
                invocation1Metadata["Product"] = "Product1";
                invocation1Metadata["Version"] = "0.0.0";

                var invocation2 = new DbInvocation { Signature = "Bar()" };
                await session.StoreAsync(invocation2, $"Product2/0.0.0/{invocation2.Signature}");

                var invocation2Metadata = session.Advanced.GetMetadataFor(invocation2);
                invocation2Metadata["Product"] = "Product2";
                invocation2Metadata["Version"] = "0.0.0";

                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new AnalysisReportManager(documentStoreProvider, configurationManager, logger);
            await sut.DeleteDbInvocationsAsync("Product1", "0.0.0");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualInvocation1 = await session.LoadAsync<DbInvocation>("Product1/0.0.0/Foo()");
                actualInvocation1.ShouldBeNull();

                var actualInvocation2 = await session.LoadAsync<DbInvocation>("Product2/0.0.0/Bar()");
                actualInvocation2.ShouldNotBeNull();
                actualInvocation2.Signature.ShouldBe("Bar()");
            }
        }
    }
}