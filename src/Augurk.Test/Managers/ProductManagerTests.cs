using System.Collections.Generic;
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
            var documentStoreProvider = DocumentStoreProvider;

            await documentStoreProvider.StoreDbFeatureAsync("Product1", "Group", "Title", "Version");
            await documentStoreProvider.StoreDbFeatureAsync("Product2", "Group", "Title", "Version");

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            var result = await sut.GetProductsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.SequenceEqual(new[] { "Product1", "Product2" }).ShouldBeTrue();
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can retrieve a product description for a particular product.
        /// </summary>
        [Fact]
        public async Task GetsProductDescription()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var product = new DbProduct
                {
                    Name = "MyProduct",
                    DescriptionMarkdown = "# Hello World!",
                };
                await session.StoreAsync(product);
                await session.SaveChangesAsync();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            var result = await sut.GetProductDescriptionAsync("MyProduct");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe("# Hello World!");
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can insert a new product description.
        /// </summary>
        [Fact]
        public async Task CanInsertProductDescription()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            var identifier = new DbProduct { Name = "MyProduct" }.GetIdentifier();

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            await sut.InsertOrUpdateProductDescriptionAsync("MyProduct", "# Hello World!");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var product = await session.LoadAsync<DbProduct>(identifier);
                product.ShouldNotBeNull();
                product.DescriptionMarkdown.ShouldBe("# Hello World!");
            }
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can update an existing product description.
        /// </summary>
        [Fact]
        public async Task CanUpdateProductDescription()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            var existingProduct = new DbProduct { Name = "MyProduct", DescriptionMarkdown = "# Hello World!" };
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(existingProduct, existingProduct.GetIdentifier());
            }

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            await sut.InsertOrUpdateProductDescriptionAsync("MyProduct", "# MyProduct");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var product = await session.LoadAsync<DbProduct>(existingProduct.GetIdentifier());
                product.ShouldNotBeNull();
                product.DescriptionMarkdown.ShouldBe("# MyProduct");
            }
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can delete features associated with a product.
        /// </summary>
        [Fact]
        public async Task CanDeleteProduct()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;

            var existingFeature1 = await documentStoreProvider.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");
            var existingFeature2 = await documentStoreProvider.StoreDbFeatureAsync("MyOtherProduct", "MyGroup", "MySecondFeature", "0.0.0");

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            await sut.DeleteProductAsync("MyProduct");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualFeature1 = await session.LoadAsync<DbFeature>(existingFeature1.GetIdentifier());
                var actualFeature2 = await session.LoadAsync<DbFeature>(existingFeature2.GetIdentifier());

                actualFeature1.ShouldBeNull();
                actualFeature2.ShouldNotBeNull();
            }
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can delete features associated with a particular product
        /// at a specific version.
        /// </summary>
        [Fact]
        public async Task CanDeleteProductVersion()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;

            var existingFeature1 = await documentStoreProvider.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0");
            var existingFeature2 = await documentStoreProvider.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "1.0.0");

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            await sut.DeleteProductAsync("MyProduct", "0.0.0");

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var actualFeature = await session.LoadAsync<DbFeature>(existingFeature1.GetIdentifier());

                actualFeature.Versions.ShouldBe(new [] {"1.0.0"});
            }
        }

        /// <summary>
        /// Tests that the <see cref="ProductManager" /> class can get all the available tags for
        /// all features.
        /// </summary>
        [Fact]
        public async Task CanGetTags()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;

            var existingFeature1 = await documentStoreProvider.StoreDbFeatureAsync("MyProduct", "MyGroup", "MyFirstFeature", "0.0.0", "tag1", "tag2");
            var existingFeature2 = await documentStoreProvider.StoreDbFeatureAsync("MyProduct", "MyGroup", "MySecondFeature", "0.0.0", "tag2", "tag3");

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ProductManager(documentStoreProvider, logger);
            var result = await sut.GetTagsAsync("MyProduct");

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
            result.SequenceEqual(new List<string> { "tag1", "tag2", "tag3" });
        }
    }
}