using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Entities;
using Xunit;
using Alba;
using System.Linq;
using Shouldly;
using System.Net;

namespace Augurk.IntegrationTest
{
    /// <summary>
    /// Contains integration tests related to upload scenario's (ie. feature files and pruduct descriptions).
    /// </summary>
    [Collection(nameof(SystemUnderTestCollection))]
    public class UploadScenarios : TestBase
    {
        private const string productName = "MyProduct";
        private const string groupName = "MyGroup";

        /// <summary>
        /// Initializes a new instance of hte <see cref="UploadScenarios" /> class.
        /// </summary>
        /// <param name="fixture">A <see cref="SystemUnderTestFixture" /> instance to use.</param>
        public UploadScenarios(SystemUnderTestFixture fixture) : base(fixture)
        {
        }

        /// <summary>
        /// Tests that a feature can be uploaded and subsequently retrieved from the API.
        /// </summary>
        [Fact]
        public async Task CanUploadFeature()
        {
            // Arrange
            var expectedFeature = new Feature
            {
                Title = "A simple feature",
                Description = "As a math idiot I want to add two numbers so that I can calculate the result"
            };

            // Act
            await System.Scenario(_ =>
            {
                _.PostFeature(expectedFeature, productName, groupName);
                _.StatusCodeShouldBe(HttpStatusCode.Accepted);
            });

            WaitForIndexing(Store);

            var result = await System.Scenario(_ =>
            {
                _.GetFeature(productName, groupName, expectedFeature.Title);
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var actualFeature = result.ResponseBody.ReadAsJson<Feature>();
            actualFeature.Title.ShouldBe(expectedFeature.Title);
            actualFeature.Description.ShouldBe(expectedFeature.Description);
        }

        /// <summary>
        /// Tests that a description for a product can be uploaded and subsequently retrieved through the API.
        /// </summary>
        [Fact]
        public async Task CanUploadProductDescription()
        {
            // Arrange
            var expectedProductDescription = "# MyProduct";

            // Act
            await System.Scenario(_ =>
            {
                _.PutProductDescription(productName, expectedProductDescription);
                _.StatusCodeShouldBeOk();
            });

            WaitForIndexing(Store);

            var result = await System.Scenario(_ =>
            {
                _.GetProductDescription(productName);
                _.StatusCodeShouldBeOk();
            });

            // Assert
            var actualProductDescription = result.ResponseBody.ReadAsText();
            actualProductDescription.ShouldBe(expectedProductDescription);
        }
    }
}