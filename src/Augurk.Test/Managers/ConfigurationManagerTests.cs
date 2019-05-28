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
    public class ConfigurationManagerTests : RavenTestBase
    {
        /// <summary>
        /// Tests that the ConfigurationManager retrieves an existing configuration.
        /// </summary>
        [Fact]
        public async Task GetsExistingConfiguration()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;
            var expectedConfiguration = new Configuration()
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                DependenciesEnabled = true,
                ExpirationRegex = @"\d"
            };

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(expectedConfiguration, "urn:Augurk:Configuration");
                await session.SaveChangesAsync();
            }

            // Act
            var sut = new ConfigurationManager(documentStoreProvider, Substitute.For<IExpirationManager>());
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
        public async Task CreatesDefaultConfigurationIfNoneExists()
        {
            // Arrange
            var documentStoreProvider = DocumentStoreProvider;

            // Act
            var sut = new ConfigurationManager(documentStoreProvider, Substitute.For<IExpirationManager>());
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
            var documentStoreProvider = DocumentStoreProvider;
            var newConfiguration = new Configuration
            {
                ExpirationEnabled = true,
                ExpirationDays = 10,
                DependenciesEnabled = true,
                ExpirationRegex = ".*"
            };
            var expirationSubstitute = Substitute.For<IExpirationManager>();

            // Act
            var sut = new ConfigurationManager(documentStoreProvider, expirationSubstitute);
            await sut.PersistConfigurationAsync(newConfiguration);

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var configuration = await session.LoadAsync<Configuration>("urn:Augurk:Configuration");
                configuration.ExpirationEnabled.ShouldBeTrue();
                configuration.ExpirationDays.ShouldBe(10);
                configuration.DependenciesEnabled.ShouldBeTrue();
                configuration.ExpirationRegex.ShouldBe(".*");
            }
            await expirationSubstitute.Received().ApplyExpirationPolicyAsync(Arg.Any<Configuration>());
        }
    }
}