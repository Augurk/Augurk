using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Api;
using Augurk.Api.Managers;
using Augurk.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Raven.Client.Documents.Session;

namespace Augurk.Test
{
    /// <summary>
    /// Contains extension methods to make tests easier to write.
    /// </summary>
    public static class TestExtensions
    {
        /// <summary>
        /// Stores a <see cref="DbFeature" /> with the provided parameters to the database.
        /// </summary>
        /// <param name="session">An <see cref="IAsyncDocumentSession" /> used to store the data.</param>
        /// <param name="product">Name of the product containing the feature.</param>
        /// <param name="group">Name of the group containing the feature.</param>
        /// <param name="title">Title of the feature.</param>
        /// <param name="version">Version of the feature.</param>
        /// <param name="tags">Optional additional tags for the feature.</param>
        public static Task StoreDbFeatureAsync(this IDocumentStoreProvider documentStoreProvider, string product, string group, string title, string version, params string[] tags)
        {
            var feature = GenerateFeature(title, tags);

            var featureManager = new FeatureManager(documentStoreProvider, Substitute.For<ILogger<FeatureManager>>());
            return featureManager.InsertOrUpdateFeatureAsync(feature, product, group, version);
        }

        /// <summary>
        /// Generates a <see cref="DbFeature" /> instance.
        /// </summary>
        /// <param name="product">Name of the product containing the feature.</param>
        /// <param name="group">Name of the group containing the feature.</param>
        /// <param name="title">Title of the feature.</param>
        /// <param name="version">Version of the feature.</param>
        /// <param name="tags">Optional additional tags for the feature.</param>
        /// <returns>A <see cref="Feature" /> instance with the provided values set.</returns>
        public static Feature GenerateFeature(string title, params string[] tags)
        {
            var feature = new Feature
            {
                Title = title
            };

            feature.Tags = new List<string>(tags);

            return feature;
        }
    }
}