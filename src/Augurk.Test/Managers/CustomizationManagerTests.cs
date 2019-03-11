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
    /// Contains unit tests for the <see cref="CustomizationManager" /> class.
    /// </summary>
    public class CustomizationManagerTests : RavenTestBase
    {
        /// <summary>
        /// Tests that the CustomizationManager retrieves an existing customization.
        /// </summary>
        [Fact]
        public async Task GetsExistingCustomization()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var expectedConfiguration = new Customization()
            {
                InstanceName = "MyCustomInstance"
            };

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                await session.StoreAsync(expectedConfiguration, "urn:Augurk:Customization");
                await session.SaveChangesAsync();
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
            var documentStoreProvider = GetDocumentStoreProvider();

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
            var documentStoreProvider = GetDocumentStoreProvider();
            var newCustomization = new Customization
            {
                InstanceName = "MyCustomInstance"
            };

            // Act
            var sut = new CustomizationManager(documentStoreProvider);
            await sut.PersistCustomizationSettingsAsync(newCustomization);

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var configuration = await session.LoadAsync<Customization>("urn:Augurk:Customization");
                configuration.InstanceName.ShouldBe("MyCustomInstance");
            }
        }
    }
}