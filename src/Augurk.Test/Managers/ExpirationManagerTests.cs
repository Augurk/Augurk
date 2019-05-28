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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Augurk.Api;
using Augurk.Api.Managers;
using Augurk.Entities;
using NSubstitute;
using Raven.Client;
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
            var configuration = new Configuration()
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"\d"
            };

            var dbFeature = new DbFeature { Version = "1.0.0" };
            DateTime expectedUploadDate = await PersistDocument("testdocument1", dbFeature);

            // Act
            var sut = new ExpirationManager(DocumentStoreProvider);
            await sut.ApplyExpirationPolicyAsync(configuration);

            WaitForIndexing(DocumentStore);

            // Assert
            await AssertMetadata("testdocument1", expectedUploadDate, expectedUploadDate.AddDays(configuration.ExpirationDays));
        }

        /// <summary>
        /// Tests that the ExpirationManager removes expiration for documents that do not match the version regex.
        /// </summary>
        [Fact]
        public async Task RemoveExpirationFromNonMatchingVersion()
        {
            // Arrange
            var configuration = new Configuration()
            {
                ExpirationEnabled = true,
                ExpirationDays = 1,
                ExpirationRegex = @"Hello World"
            };

            var dbFeature = new DbFeature { Version = "1.0.0" };
            var additionalMetadata = new Dictionary<string, string> { { Constants.Documents.Metadata.Expires, DateTime.UtcNow.ToString() } };
            var expectedUploadDate = await PersistDocument("testdocument1", dbFeature, additionalMetadata);

            // Act
            var sut = new ExpirationManager(DocumentStoreProvider);
            await sut.ApplyExpirationPolicyAsync(configuration);

            WaitForIndexing(DocumentStore);

            // Assert
            await AssertMetadata("testdocument1", expectedUploadDate, null);
        }

        public async Task SetUploadDateOnNonMatchingVersion()
        {

        }

        [Fact]
        public async Task RemoveExpirationWhenDisabled() 
        {
            // Arrange
            var configuration = new Configuration()
            {
                ExpirationEnabled = false,
                ExpirationDays = 1,
                ExpirationRegex = @"\d"
            };

            var dbFeature = new DbFeature { Version = "1.0.0" };
            var additionalMetadata = new Dictionary<string, string> { { Constants.Documents.Metadata.Expires, DateTime.UtcNow.ToString() } };
            var expectedUploadDate = await PersistDocument("testdocument1", dbFeature, additionalMetadata);

            // Act
            var sut = new ExpirationManager(DocumentStoreProvider);
            await sut.ApplyExpirationPolicyAsync(configuration);

            WaitForIndexing(DocumentStore);

            // Assert
            await AssertMetadata("testdocument1", expectedUploadDate, null);
        }

        [Fact]
        public async Task SetUploadDateOnNewDocumentsWhenDisabled() 
        {
            // Arrange
            var configuration = new Configuration()
            {
                ExpirationEnabled = false,
                ExpirationDays = 1,
                ExpirationRegex = @"\d"
            };

            var dbFeature = new DbFeature { Version = "1.0.0" };
            DateTime expectedUploadDate = await PersistDocument("testdocument1", dbFeature);

            // Act
            var sut = new ExpirationManager(DocumentStoreProvider);
            await sut.ApplyExpirationPolicyAsync(configuration);

            WaitForIndexing(DocumentStore);


            // Assert
            await AssertMetadata("testdocument1", expectedUploadDate, null);
        }

        public async Task DoNotSetExpirationOnNonVersionedDocuments() { }

        public async Task DoNotRemoveExpirationFromNonVersionedDocuments() { }

        private DateTime ParseWithoutMilliseconds(string dateString)
        {
            var result = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Minute, result.Second);
            return result;
        }


        private async Task<DateTime> PersistDocument(string documentId, object document, Dictionary<string, string> additionalMetadata = null)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                await session.StoreAsync(document, documentId);
                await session.SaveChangesAsync();

                if (additionalMetadata != null)
                {
                    var documentMetadata = session.Advanced.GetMetadataFor(document);
                    foreach (var pair in additionalMetadata)
                    {
                        documentMetadata[pair.Key] = pair.Value;
                    }
                }


                WaitForIndexing(DocumentStore);

                var metadata = session.Advanced.GetMetadataFor(document);
                return ParseWithoutMilliseconds(metadata[Constants.Documents.Metadata.LastModified].ToString());
            }
        }

        private async Task AssertMetadata(string documentId, DateTime? expectedUploadDate, DateTime? expectedExpireDate)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var document = await session.LoadAsync<DbFeature>(documentId);
                var metadata = session.Advanced.GetMetadataFor(document);
                if (expectedUploadDate.HasValue)
                {
                    metadata.ContainsKey("upload-date").ShouldBeTrue();
                    DateTime uploadDate = ParseWithoutMilliseconds(metadata["upload-date"].ToString());
                    uploadDate.ShouldBe(expectedUploadDate.Value);
                }
                else
                {
                    metadata.ContainsKey("upload-date").ShouldBeFalse();
                }
                if (expectedExpireDate.HasValue)
                {

                    metadata.ContainsKey(Constants.Documents.Metadata.Expires).ShouldBeTrue();
                    DateTime expires = ParseWithoutMilliseconds(metadata[Constants.Documents.Metadata.Expires].ToString());
                    expires.ShouldBe(expectedExpireDate.Value);
                }
                else
                {
                    metadata.ContainsKey(Constants.Documents.Metadata.Expires).ShouldBeFalse();
                }
            }
        }
    }
}