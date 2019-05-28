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
using Augurk.Api;
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
    /// Contains unit tests for the <see cref="ExpirationManager" /> class.
    /// </summary>
    public class ExpirationManagerTests : RavenTestBase
    {
        /// <summary>
        /// Tests that the ExpirationManager sets the expiration properly.
        /// </summary>
        [Fact]
        public async Task SetExpiration()
        {
            // Arrange
            var documentStoreProvider = GetDocumentStoreProvider();
            var configuration = new Configuration()
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"\d"
            };
            string expectedUploadDate;

            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var dbFeature = new DbFeature {Version = "1.0.0"};
                await session.StoreAsync(dbFeature, "testdocument1");
                await session.SaveChangesAsync();
                var metadata = session.Advanced.GetMetadataFor(dbFeature);
                expectedUploadDate = metadata["@last-modified"].ToString();
            }

            WaitForIndexing(documentStoreProvider.Store);

            // Act
            var sut = new ExpirationManager(documentStoreProvider);
            await sut.ApplyExpirationPolicyAsync(configuration);

            WaitForIndexing(documentStoreProvider.Store);

            // Assert
            using (var session = documentStoreProvider.Store.OpenAsyncSession())
            {
                var document = await session.LoadAsync<DbFeature>("testdocument1");
                var metadata = session.Advanced.GetMetadataFor(document);
                metadata["upload-date"].ShouldBe(expectedUploadDate);
                metadata["@expires"].ShouldNotBeNull();
            }
        }
    }
}