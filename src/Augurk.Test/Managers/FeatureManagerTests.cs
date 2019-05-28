using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api;
using Augurk.Api.Indeces;
using Augurk.Api.Managers;
using Augurk.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client;
using Shouldly;
using Xunit;
using Augurk.Entities.Test;
using System;

namespace Augurk.Test.Managers
{
    /// <summary>
    /// Contains tests for the <see cref="FeatureManager" /> class.
    /// </summary>
    public class FeatureManagerTests : RavenTestBase
    {
        private readonly IConfigurationManager configurationManager = Substitute.For<IConfigurationManager>();
        private readonly ILogger<FeatureManager> logger = Substitute.For<ILogger<FeatureManager>>();

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> can get the available versions of a particular feature.
        /// </summary>
        [Fact]
        public async Task GetsAvailableVersionsForFeature()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "1.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "2.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetFeatureAvailableVersions("MyProduct", "MyGroup", "MyFirstFeature");

            // Assert
            result.ShouldNotBeNull();
            result.SequenceEqual(new[] { "2.0.0", "1.0.0" }).ShouldBeTrue();
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can retrieve a previously stored feature.
        /// </summary>
        [Fact]
        public async Task GetsPreviouslyStoredVersion()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");
                await session.SaveChangesAsync();
            }

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var actualFeature = await sut.GetFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");

            // Assert
            actualFeature.ShouldNotBeNull();
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can get a list of groups containing the features.
        /// </summary>
        [Fact]
        public async Task CanGetGroupedFeatureDescriptions()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MyFirstFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MySecondFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "Group2", "MyOtherFeature", "0.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetGroupedFeatureDescriptionsAsync("MyProduct");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);

            var firstGroup = result.FirstOrDefault();
            firstGroup.ShouldNotBeNull();
            firstGroup.Features.Count().ShouldBe(2);
            firstGroup.Features.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");
            firstGroup.Features.LastOrDefault()?.Title.ShouldBe("MySecondFeature");

            var secondGroup = result.LastOrDefault();
            secondGroup.ShouldNotBeNull();
            secondGroup.Features.Count().ShouldBe(1);
            secondGroup.Features.FirstOrDefault()?.Title.ShouldBe("MyOtherFeature");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can get both groups when there's a single feature
        /// under multiple groups.
        /// </summary>
        [Fact]
        public async Task CanGetGroupedFeatureDescriptions_WithSameFeatureUnderMultipleGroups()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MyFirstFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "Group2", "MyFirstFeature", "0.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetGroupedFeatureDescriptionsAsync("MyProduct");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);

            var firstGroup = result.FirstOrDefault();
            firstGroup.ShouldNotBeNull();
            firstGroup.Features.Count().ShouldBe(1);
            firstGroup.Features.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");

            var secondGroup = result.LastOrDefault();
            secondGroup.ShouldNotBeNull();
            secondGroup.Features.Count().ShouldBe(1);
            secondGroup.Features.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class does not duplicate features if multiple versions
        /// of the same feature exist.
        /// </summary>
        [Fact]
        public async Task CanGetGroupedFeatureDescriptions_DoesNotReturnSameFeatureIfMultipleVersionsExist()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MyFirstFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MyFirstFeature", "1.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetGroupedFeatureDescriptionsAsync("MyProduct");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);

            var firstGroup = result.FirstOrDefault();
            firstGroup.ShouldNotBeNull();
            firstGroup.Features.Count().ShouldBe(1);
            firstGroup.Features.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can get feature descriptions based on a branch and a tag.
        /// </summary>
        [Fact]
        public async Task CanGetFeatureDescriptionsByBranchAndTagAsync()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByProductAndBranch());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MyFirstFeature", "0.0.0", "tag1");
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MySecondFeature", "0.0.0", "tag1", "tag2");
                await session.StoreDbFeatureAsync("MyProduct", "Group2", "MyOtherFeature", "0.0.0", "tag3");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetFeatureDescriptionsByBranchAndTagAsync("MyProduct", "tag1");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");
            result.LastOrDefault()?.Title.ShouldBe("MySecondFeature");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can get feature descriptions based on a product and group.
        /// </summary>
        [Fact]
        public async Task CanGetFeatureDescriptionsByProductAndGroupAsync()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "Group1", "MyFirstFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "Group2", "MySecondFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyOtherProduct", "Group1", "MyThirdFeature", "0.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetFeatureDescriptionsByProductAndGroupAsync("MyProduct", "Group1");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(1);
            result.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can get features based on a product and version.
        /// </summary>
        [Fact]
        public async Task CanGetDbFeaturesByProductAndVersionAsync()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "1.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MySecondFeature", "1.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetDbFeaturesByProductAndVersionAsync("MyProduct", "1.0.0");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);

            result.FirstOrDefault()?.Title.ShouldBe("MyFirstFeature");
            result.FirstOrDefault()?.Version.ShouldBe("1.0.0");

            result.LastOrDefault()?.Title.ShouldBe("MySecondFeature");
            result.LastOrDefault()?.Version.ShouldBe("1.0.0");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can get all features from the database.
        /// </summary>
        [Fact]
        public async Task CanGetAllDbFeatures()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");
                await session.StoreDbFeatureAsync("MyOtherProduct", "MyOtherGroup", "MySecondFeature", "0.0.0");
                await session.SaveChangesAsync();
            }

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.GetAllDbFeatures();

            // Assert
            result.ShouldNotBeNull();
            result.FirstOrDefault().Title.ShouldBe("MyFirstFeature");
            result.LastOrDefault().Title.ShouldBe("MySecondFeature");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can persist a range of <see cref="DbFeature" /> instances.
        /// </summary>
        [Fact]
        public async Task CanPersistDbFeatures()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var expectedFeature1 = TestExtensions.GenerateDbFeature("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");
            var expectedFeature2 = TestExtensions.GenerateDbFeature("MyProduct", "MyGroup", "MySecondFeature", "0.0.0");

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            await sut.PersistDbFeatures(new[] { expectedFeature1, expectedFeature2 });

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualFeatures = await session.Query<DbFeature>().ToListAsync();
                actualFeatures.ShouldNotBeNull();
                actualFeatures.Count.ShouldBe(2);

                var actualFeature1 = actualFeatures.FirstOrDefault();
                actualFeature1.ShouldNotBeNull();
                actualFeature1.GetIdentifier().ShouldBe(expectedFeature1.GetIdentifier());

                var actualFeature2 = actualFeatures.LastOrDefault();
                actualFeature2.ShouldNotBeNull();
                actualFeature2.GetIdentifier().ShouldBe(expectedFeature2.GetIdentifier());
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can insert a new feature.
        /// </summary>
        [Fact]
        public async Task CanInsertNewFeature()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var feature = new Feature
            {
                Title = "My Feature",
                Description = "As a math idiot",
            };
            
            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.InsertOrUpdateFeatureAsync(feature, "MyProduct", "MyGroup", "0.0.0");

            // Assert
            result.Product.ShouldBe("MyProduct");
            result.Group.ShouldBe("MyGroup");
            result.Title.ShouldBe("My Feature");
            result.Version.ShouldBe("0.0.0");

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var dbFeature = session.LoadAsync<DbFeature>(result.GetIdentifier());
                dbFeature.ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class sets the expiration on a feature if that is configured.
        /// </summary>
        [Fact]
        public async Task SetsExpirationIfEnabledWhenInsertingNewFeature()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var feature = new Feature
            {
                Title = "My Feature",
                Description = "As a math idiot",
            };

            configurationManager.GetOrCreateConfigurationAsync().Returns(new Configuration
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"\d\.\d\.\d"
            });
            
            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.InsertOrUpdateFeatureAsync(feature, "MyProduct", "MyGroup", "0.0.0");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var dbFeature = await session.LoadAsync<DbFeature>(result.GetIdentifier());
                dbFeature.ShouldNotBeNull();

                var dbFeatureMetadata = session.Advanced.GetMetadataFor(dbFeature);
                dbFeatureMetadata[Constants.Documents.Metadata.Expires].ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can update an existing feature.
        /// </summary>
        [Fact]
        public async Task CanUpdateExistingFeature()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var feature = new Feature
            {
                Title = "My Feature",
                Description = "As a math idiot",
                Tags = new List<string> { "tag1", "tag2" }
            };

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My Feature", "0.0.0", "tag1");
                await session.SaveChangesAsync();
            }
            
            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.InsertOrUpdateFeatureAsync(feature, "MyProduct", "MyGroup", "0.0.0");

            // Assert
            result.Product.ShouldBe("MyProduct");
            result.Group.ShouldBe("MyGroup");
            result.Title.ShouldBe("My Feature");
            result.Version.ShouldBe("0.0.0");
            result.Tags.ShouldBe(new string[] { "tag1", "tag2" });

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var dbFeature = session.LoadAsync<DbFeature>(result.GetIdentifier());
                dbFeature.ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class sets the expiration on a feature if that is configured.
        /// </summary>
        [Fact]
        public async Task SetsExpirationIfEnabledWhenUpdatingExistingFeature()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var feature = new Feature
            {
                Title = "My Feature",
                Description = "As a math idiot",
                Tags = new List<string> { "tag1", "tag2" }
            };

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My Feature", "0.0.0", "tag1");
                await session.SaveChangesAsync();
            }

            configurationManager.GetOrCreateConfigurationAsync().Returns(new Configuration
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"\d\.\d\.\d"
            });
            
            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var result = await sut.InsertOrUpdateFeatureAsync(feature, "MyProduct", "MyGroup", "0.0.0");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var dbFeature = await session.LoadAsync<DbFeature>(result.GetIdentifier());
                dbFeature.ShouldNotBeNull();

                var dbFeatureMetadata = session.Advanced.GetMetadataFor(dbFeature);
                dbFeatureMetadata[Constants.Documents.Metadata.Expires].ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can persist test results.
        /// </summary>
        [Fact]
        public async Task CanPersistTestResults()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var expectedTestResults = new FeatureTestResult
            {
                FeatureTitle = "My Feature",
                Result = TestResult.Passed,
                TestExecutionDate = DateTime.Now
            };

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My Feature", "0.0.0");
                await session.SaveChangesAsync();
            }

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            await sut.PersistFeatureTestResultAsync(expectedTestResults, "MyProduct", "MyGroup", "0.0.0");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var dbFeature = await session.LoadAsync<DbFeature>(DbFeatureExtensions.GetIdentifier("MyProduct", "MyGroup", "My Feature", "0.0.0"));
                dbFeature.ShouldNotBeNull();
                dbFeature.TestResult.ShouldNotBeNull();
                dbFeature.TestResult.Result.ShouldBe(TestResult.Passed);
                dbFeature.TestResult.TestExecutionDate.ShouldBe(expectedTestResults.TestExecutionDate);
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class throws an exception when attempting to store test results
        /// for a feature that hasn't been previously stored.
        /// </summary>
        [Fact]
        public async Task ThrowsWhenStoringTestResultsForNonExistingFeature()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var expectedTestResults = new FeatureTestResult
            {
                FeatureTitle = "My Feature",
                Result = TestResult.Passed,
                TestExecutionDate = DateTime.Now
            };

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            var exception = await Should.ThrowAsync<Exception>(sut.PersistFeatureTestResultAsync(expectedTestResults, "MyProduct", "MyGroup", "0.0.0"));

            // Assert
            exception.Message.ShouldContain(expectedTestResults.FeatureTitle);
            exception.Message.ShouldContain("MyProduct");
            exception.Message.ShouldContain("MyGroup");
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can delete features based on a product and group.
        /// </summary>
        [Fact]
        public async Task CanDeleteFeaturesByProductAndGroup()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyOtherGroup", "My Second Feature", "0.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            await sut.DeleteFeaturesAsync("MyProduct", "MyGroup");
            
            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var features = await session.Query<DbFeature>().ToListAsync();
                features.ShouldNotBeNull();
                features.Count.ShouldBe(1);
                
                var remainingFeature = features.FirstOrDefault();
                remainingFeature.ShouldNotBeNull();
                remainingFeature.Title.ShouldBe("My Second Feature");
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can delete features based on a product, group and version.
        /// </summary>
        [Fact]
        public async Task CanDeleteFeaturesByProductGroupAndVersion()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "1.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            await sut.DeleteFeaturesAsync("MyProduct", "MyGroup", "0.0.0");
            
            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var features = await session.Query<DbFeature>().ToListAsync();
                features.ShouldNotBeNull();
                features.Count.ShouldBe(1);
                
                var remainingFeature = features.FirstOrDefault();
                remainingFeature.ShouldNotBeNull();
                remainingFeature.Version.ShouldBe("1.0.0");
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can delete a feature based on a product, group and title.
        /// </summary>
        [Fact]
        public async Task CanDeleteFeatureByProductGroupAndTitle()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            await documentStoreProvider.Store.ExecuteIndexAsync(new Features_ByTitleProductAndGroup());

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "1.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My Second Feature", "0.0.0");
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            await sut.DeleteFeatureAsync("MyProduct", "MyGroup", "My First Feature");
            
            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var features = await session.Query<DbFeature>().ToListAsync();
                features.ShouldNotBeNull();
                features.Count.ShouldBe(1);
                
                var remainingFeature = features.FirstOrDefault();
                remainingFeature.ShouldNotBeNull();
                remainingFeature.Title.ShouldBe("My Second Feature");
            }
        }

        /// <summary>
        /// Tests that the <see cref="FeatureManager" /> class can delete a feature based on a product, group, title and version.
        /// </summary>
        [Fact]
        public async Task CanDeleteFeatureByProductGroupTitleAndVersion()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "0.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My First Feature", "1.0.0");
                await session.StoreDbFeatureAsync("MyProduct", "MyGroup", "My Second Feature", "0.0.0");
                await session.SaveChangesAsync();
            }

            // Act
            var sut = new FeatureManager(documentStoreProvider, configurationManager, logger);
            await sut.DeleteFeatureAsync("MyProduct", "MyGroup", "My First Feature", "0.0.0");
            
            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var features = await session.Query<DbFeature>().ToListAsync();
                features.ShouldNotBeNull();
                features.Count.ShouldBe(2);
                
                var remainingFeature1 = features.FirstOrDefault();
                remainingFeature1.ShouldNotBeNull();
                remainingFeature1.Title.ShouldBe("My First Feature");
                remainingFeature1.Version.ShouldBe("1.0.0");

                var remainingFeature2 = features.LastOrDefault();
                remainingFeature2.ShouldNotBeNull();
                remainingFeature2.Title.ShouldBe("My Second Feature");
                remainingFeature2.Version.ShouldBe("0.0.0");
            }
        }
    }
}