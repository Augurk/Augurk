using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api;
using Augurk.Api.Indeces;
using Augurk.Api.Managers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Raven.Client.Documents.Session;
using Shouldly;
using Xunit;

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
    }
}