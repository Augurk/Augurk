using System.Threading.Tasks;
using Augurk.Api.Managers;
using Augurk.Entities;
using NSubstitute;
using Raven.Client.Documents;
using Raven.TestDriver;
using Shouldly;
using Xunit;

namespace Augurk.Test.Managers
{
    /// <summary>
    /// Contains unit tests for the <see cref="CustomizationManager" /> class.
    /// </summary>
    public class CustomizationManagerTests : RavenTestDriver
    {
        private readonly IDocumentStoreProvider documentStoreProvider = Substitute.For<IDocumentStoreProvider>();

        /// <summary>
        /// Called before the document store is being initialized.
        /// </summary>
        /// <param name="documentStore">A <see cref="IDocumentStore" /> instance to configure.</param>
        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.Conventions.IdentityPartsSeparator = "-";
        }

        /// <summary>
        /// Tests that the CustomizationManager retrieves an existing customization.
        /// </summary>
        [Fact]
        public async Task GetsExistingCustomization()
        {
            // Arrange
            var store = GetDocumentStore();
            documentStoreProvider.Store.Returns(store);

            var expectedConfiguration = new Customization()
            {
                InstanceName = "MyCustomInstance"
            };

            using (var session = store.OpenSession())
            {
                session.Store(expectedConfiguration, "urn:Augurk:Customization");
                session.SaveChanges();
            }

            // Act
            var sut = new CustomizationManager(documentStoreProvider);
            var result = await sut.GetOrCreateCustomizationSettingsAsync();

            // Assert
            result.InstanceName.ShouldBe("MyCustomInstance");
        }

        /// <summary>
        /// Tests that the CustomizationManager creates a default customization if no customization exists.
        /// </summary>
        [Fact]
        public async Task CreatesDefaultCustomizationIfNoneExists()
        {
            // Arrange
            var store = GetDocumentStore();
            documentStoreProvider.Store.Returns(store);

            // Act
            var sut = new CustomizationManager(documentStoreProvider);
            var result = await sut.GetOrCreateCustomizationSettingsAsync();

            // Assert
            result.InstanceName.ShouldBe("Augurk");
        }

        /// <summary>
        /// Tests that the CustomizationManager can persist a new customization.
        /// </summary>
        [Fact]
        public async Task CanPersistCustomization()
        {
            // Arrange
            var store = GetDocumentStore();
            documentStoreProvider.Store.Returns(store);

            var newCustomization = new Customization
            {
                InstanceName = "MyCustomInstance"
            };

            // Act
            var sut = new CustomizationManager(documentStoreProvider);
            await sut.PersistCustomizationSettingsAsync(newCustomization);

            // Assert
            using (var session = store.OpenSession())
            {
                var configuration = session.Load<Customization>("urn:Augurk:Customization");
                configuration.InstanceName.ShouldBe("MyCustomInstance");
            }
        }
    }
}