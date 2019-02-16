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
    /// Contains unit tests for the <see cref="ConfigurationManager" /> class.
    /// </summary>
    public class ConfigurationManagerTests : RavenTestDriver
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
        /// Tests that the ConfigurationManaager retrieves an existing configuration.
        /// </summary>
        [Fact]
        public async Task GetsExistingConfiguration()
        {
            // Arrange
            var store = GetDocumentStore();
            documentStoreProvider.Store.Returns(store);

            var expectedConfiguration = new Configuration()
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                DependenciesEnabled = true,
                ExpirationRegex = @"\d"
            };

            using (var session = store.OpenSession())
            {
                session.Store(expectedConfiguration, "urn:Augurk:Configuration");
                session.SaveChanges();
            }

            // Act
            var sut = new ConfigurationManager(documentStoreProvider);
            var result = await sut.GetOrCreateConfigurationAsync();

            // Assert
            result.ExpirationEnabled.ShouldBe(true);
            result.ExpirationDays.ShouldBe(1);
            result.DependenciesEnabled.ShouldBe(true);
            result.ExpirationRegex.ShouldBe(@"\d");
        }

        /// <summary>
        /// Tests that the ConfigurationManager creates a default configuration if no configuration exists.
        /// </summary>
        [Fact]
        public async Task CreatesDefaultConfigurationIfNonExists()
        {
            // Arrange
            var store = GetDocumentStore();
            documentStoreProvider.Store.Returns(store);

            // Act
            var sut = new ConfigurationManager(documentStoreProvider);
            var result = await sut.GetOrCreateConfigurationAsync();

            // Assert
            result.ExpirationEnabled.ShouldBe(false);
            result.ExpirationDays.ShouldBe(30);
            result.DependenciesEnabled.ShouldBe(false);
            result.ExpirationRegex.ShouldBe("[0-9.]+-.*");
        }

        /// <summary>
        /// Tests that the ConfigurationManager can persist a new configuration.
        /// </summary>
        [Fact]
        public async Task CanPersistConfiguration()
        {
            // Arrange
            var store = GetDocumentStore();
            documentStoreProvider.Store.Returns(store);

            var newConfiguration = new Configuration
            {
                ExpirationEnabled = true,
                ExpirationDays = 10,
                DependenciesEnabled = true,
                ExpirationRegex = ".*"
            };

            // Act
            var sut = new ConfigurationManager(documentStoreProvider);
            await sut.PersistConfigurationAsync(newConfiguration);

            // Assert
            using (var session = store.OpenSession())
            {
                var configuration = session.Load<Configuration>("urn:Augurk:Configuration");
                configuration.ExpirationEnabled.ShouldBeTrue();
                configuration.ExpirationDays.ShouldBe(10);
                configuration.DependenciesEnabled.ShouldBeTrue();
                configuration.ExpirationRegex.ShouldBe(".*");
            }
        }
    }
}