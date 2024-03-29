using System.Threading.Tasks;
using Augurk.Entities;
using Xunit;
using Alba;
using Shouldly;
using System.Net;
using System.IO;
using System.Net.Http;

namespace Augurk.IntegrationTest
{
    /// <summary>
    /// Contains integration tests related to import/export scenario's.
    /// </summary>
    [Collection(nameof(SystemUnderTestCollection))]
    public class ImportExportScenarios : TestBase
    {
        /// <summary>
        /// Initializes a new instance of hte <see cref="ImportExportScenarios" /> class.
        /// </summary>
        /// <param name="fixture">A <see cref="SystemUnderTestFixture" /> instance to use.</param>
        public ImportExportScenarios(SystemUnderTestFixture fixture) : base(fixture)
        {
        }

        /// <summary>
        /// Tests that am export can be created using the API.
        /// </summary>
        [Fact]
        public async Task CanExport()
        {
            // Arrange
            var feature = new Feature
            {
                Title = "A simple feature",
                Description = "As a math idiot I want to add two numbers so that I can calculate the result"
            };

            // Act
            await System.Scenario(_ =>
            {
                _.PostFeature(feature, "MyProduct", "MyGroup");
                _.StatusCodeShouldBe(HttpStatusCode.Accepted);
            });

            var result = await System.Scenario(_ =>
            {
                _.Get.Url("/api/v2/export");
                _.ContentTypeShouldBe("application/octet-stream");
                _.Header("Content-Disposition").ShouldHaveOneNonNullValue();
                _.StatusCodeShouldBeOk();
            });

            // Assert
            // Assertion already handled above
        }

        /// <summary>
        /// Tests that a file can be imported through the API.
        /// </summary>
        [Fact]
        public async Task CanImport()
        {
            // Arrange
            var fileName = "augurk-documentation.bak";
            using var file = File.OpenRead(Path.Combine("../../../", fileName));
            using var content = new StreamContent(file);
            using var formData = new MultipartFormDataContent
            {
                { content, "file", fileName }
            };

            // Act
            var client = System.Server.CreateClient();
            var response = await client.PostAsync("/api/v2/import", formData);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            WaitForIndexing(Store);

            // Assert
            var result = await System.Scenario(_ =>
            {
                _.GetProductDescription("Documentation");
                _.StatusCodeShouldBeOk();
            });
        }
    }
}