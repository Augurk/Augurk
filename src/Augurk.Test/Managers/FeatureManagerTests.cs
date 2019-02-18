using System.Linq;
using System.Threading.Tasks;
using Augurk.Api;
using Augurk.Api.Indeces;
using Augurk.Api.Managers;
using Microsoft.Extensions.Logging;
using NSubstitute;
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

            var featureV1 = GenerateDbFeature("MyProduct", "MyGroup", "MyFirstFeature", "1.0.0");
            var featureV2 = GenerateDbFeature("MyProduct", "MyGroup", "MyFirstFeature", "2.0.0");

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(featureV1);
                await session.StoreAsync(featureV2);
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
            var expectedFeatue = GenerateDbFeature("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(expectedFeatue, expectedFeatue.GetIdentifier());
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

            var feature1 = GenerateDbFeature("MyProduct", "Group1", "MyFirstFeature", "0.0.0");
            var feature2 = GenerateDbFeature("MyProduct", "Group1", "MySecondFeature", "0.0.0");
            var feature3 = GenerateDbFeature("MyProduct", "Group2", "MyOtherFeature", "0.0.0");

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(feature1, feature1.GetIdentifier());
                await session.StoreAsync(feature2, feature2.GetIdentifier());
                await session.StoreAsync(feature3, feature3.GetIdentifier());
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

        private static DbFeature GenerateDbFeature(string product, string group, string title, string version)
        {
            return new DbFeature
            {
                Product = product,
                Group = group,
                Title = title,
                Version = version
            };
        }
    }
}