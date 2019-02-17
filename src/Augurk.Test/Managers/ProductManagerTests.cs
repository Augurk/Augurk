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
    /// Contains tests for the <see cref="ProductManager" /> class.
    /// </summary>
    public class ProductManagerTests : RavenTestBase
    {
        private readonly ILogger<ProductManager> logger = Substitute.For<ILogger<ProductManager>>();

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> can retrieved stored products.
        /// </summary>
        [Fact]
        public async Task GetsProducts()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            documentStoreProvider.Store.ExecuteIndex(new Features_ByTitleProductAndGroup());
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(new DbFeature { Product = "Product1" });
                await session.StoreAsync(new DbFeature { Product = "Product2" });
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            var result = await sut.GetProductsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.SequenceEqual(new [] { "Product1", "Product2" }).ShouldBeTrue();
        }
    }
}