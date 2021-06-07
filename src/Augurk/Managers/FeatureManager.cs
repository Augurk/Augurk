// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api.Indeces;
using Augurk.Entities;
using Augurk.Entities.Test;
using Newtonsoft.Json;
using System.Collections.Generic;
using Group = Augurk.Entities.Group;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Microsoft.Extensions.Logging;
using Augurk.Entities.Search;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve features from storage.
    /// </summary>
    public class FeatureManager : IFeatureManager
    {
        private readonly IDocumentStoreProvider _storeProvider;
        private readonly ILogger<FeatureManager> _logger;

        /// <summary>
        /// Gets or sets the JsonSerializerSettings that should be used when (de)serializing.
        /// </summary>
        internal static JsonSerializerSettings JsonSerializerSettings { get; set; }

        public FeatureManager(IDocumentStoreProvider storeProvider, ILogger<FeatureManager> logger)
        {
            _storeProvider = storeProvider ?? throw new ArgumentNullException(nameof(storeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the available versions of a particular feature.
        /// </summary>
        /// <param name="productName">Name of the product the feature belongs to.</param>
        /// <param name="groupName">Name of the group the product belongs to.</param>
        /// <param name="title">Title of the feature to get the versions for.</param>
        /// <returns>Returns a range of available versions.</returns>
        public async Task<IEnumerable<string>> GetFeatureAvailableVersions(string productName, string groupName, string title)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var features = await session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                        .Where(feature => feature.Product == productName && feature.Group == groupName && feature.Title == title)
                                        .OfType<DbFeature>()

                                        .ToListAsync();

            return features.SelectMany(f => f.Versions).OrderByDescending(version => version, new SemanticVersionComparer());
        }

        /// <summary>
        /// Gets the feature that matches the provided criteria.
        /// </summary>
        /// <param name="productName">The name of the product under which the feature is positioned.</param>
        /// <param name="groupName">The name of the group under which the feature is positioned.</param>
        /// <param name="title">The title of the feature.</param>
        /// <param name="version">Version of the feature to retrieve.</param>
        /// <returns>
        /// A <see cref="DisplayableFeature"/> instance describing the requested feature;
        /// or <c>null</c> if the feature cannot be found.
        /// </returns>
        public async Task<DisplayableFeature> GetFeatureAsync(string productName, string groupName, string title, string version)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var dbFeature = await session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                         .Where(feature => feature.Product == productName
                                                        && feature.Group == groupName
                                                        && feature.Title == title
                                                        && feature.Version == version)
                                         .OfType<DbFeature>()
                                         .SingleOrDefaultAsync();

            if (dbFeature == null)
            {
                return null;
            }

            var result = new DisplayableFeature(dbFeature)
            {
                TestResult = dbFeature.TestResult,
                Version = version
            };

            // Process the server tags
            var processor = new FeatureProcessor();
            processor.Process(result);

            return result;
        }

        /// <summary>
        /// Gets groups containing the descriptions for all features for the specified product.
        /// </summary>
        /// <param name="productName">The name of the product for which the feature descriptions should be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="Group"/> instances.</returns>
        public async Task<IEnumerable<Group>> GetGroupedFeatureDescriptionsAsync(string productName)
        {
            var featureDescriptions = new Dictionary<string, List<FeatureDescription>>();
            var groups = new Dictionary<string, Group>();

            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                var data = await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                        .Where(feature => feature.Product.Equals(productName, StringComparison.OrdinalIgnoreCase))
                                        .Select(feature =>
                                                new
                                                {
                                                    feature.Group,
                                                    feature.ParentTitle,
                                                    feature.Title,
                                                    feature.Versions
                                                })
                                        .Take(1000)
                                        .ToListAsync();

                var comparer = new SemanticVersionComparer();
                foreach (var uniqueFeature in data.GroupBy(record => (group: record.Group, title: record.Title)))
                {
                    var latestFeature = uniqueFeature.OrderByDescending(record => record.Versions.OrderByDescending(v => v, comparer).First(), comparer).First();
                    var featureDescription = new FeatureDescription()
                    {
                        Title = uniqueFeature.Key.title,
                        LatestVersion = latestFeature.Versions.OrderByDescending(v => v, comparer).First(),
                    };

                    if (string.IsNullOrWhiteSpace(latestFeature.ParentTitle))
                    {
                        if (!groups.ContainsKey(latestFeature.Group))
                        {
                            // Create a new group
                            groups.Add(latestFeature.Group, new Group()
                            {
                                Name = latestFeature.Group,
                                Features = new List<FeatureDescription>()
                            });
                        }

                        // Add the feature to the group
                        ((List<FeatureDescription>)groups[latestFeature.Group].Features).Add(featureDescription);
                    }
                    else
                    {
                        if (!featureDescriptions.ContainsKey(latestFeature.ParentTitle))
                        {
                            featureDescriptions.Add(latestFeature.ParentTitle, new List<FeatureDescription>());
                        }

                        featureDescriptions[latestFeature.ParentTitle].Add(featureDescription);
                    }
                }

                // Map the lower levels
                foreach (var feature in groups.Values.SelectMany(group => group.Features))
                {
                    AddChildren(feature, featureDescriptions);
                }
            }

            return groups.Values.OrderBy(group => group.Name).ToList();
        }

        /// <summary>
        /// Gets a collection of features for the specified <paramref name="branchName">branch</paramref> and tag.
        /// </summary>
        /// <param name="branchName">The name of the branch for which the feature descriptions should be retrieved.</param>
        /// <param name="tag">A tag which should be used to filter the results.</param>
        /// <returns>An enumerable collection of <see cref="FeatureDescription"/> instances.</returns>
        public async Task<IEnumerable<FeatureDescription>> GetFeatureDescriptionsByBranchAndTagAsync(string branchName, string tag)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var titles = await session.Query<Features_ByProductAndBranch.TaggedFeature, Features_ByProductAndBranch>()
                                    .Where(feature => feature.Product.Equals(branchName, StringComparison.OrdinalIgnoreCase)
                                                   && feature.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase))
                                    .Select(feature =>
                                            new
                                            {
                                                feature.Title
                                            })
                                    .ToListAsync();

            return titles.Select(feature => new FeatureDescription() { Title = feature.Title });
        }

        /// <summary>
        /// Gets a collection of features for the specified <paramref name="productName">product</paramref> and <paramref name="groupName">group</paramref>.
        /// </summary>
        /// <param name="productName">The name of the product for which the feature descriptions should be retrieved.</param>
        /// <param name="groupName">The group which should be used to filter the results.</param>
        /// <returns>An enumerable collection of <see cref="FeatureDescription"/> instances.</returns>
        public async Task<IEnumerable<FeatureDescription>> GetFeatureDescriptionsByProductAndGroupAsync(string productName, string groupName)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var titles = await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                    .Where(feature => feature.Product.Equals(productName, StringComparison.OrdinalIgnoreCase)
                                                   && feature.Group.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                                    .Select(feature =>
                                            new
                                            {
                                                feature.Title
                                            })
                                    .ToListAsync();

            return titles.Select(feature => new FeatureDescription() { Title = feature.Title });
        }

        /// <summary>
        /// Gets a collection of <see cref="DbFeature"/> instances that match the
        /// provided <paramref name="productName"/> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">The name of the product for which the features should be retrieved.</param>
        /// <param name="version">The version of the product for which the features should be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="DbFeature"/> instances.</returns>
        public async Task<IEnumerable<DbFeature>> GetDbFeaturesByProductAndVersionAsync(string productName, string version)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var featureQuery = session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                        .Where(feature => feature.Product.Equals(productName, StringComparison.OrdinalIgnoreCase)
                                                       && feature.Version.Equals(version, StringComparison.OrdinalIgnoreCase))
                                        .OfType<DbFeature>();

            return await featureQuery.ToListAsync();
        }

        /// <summary>
        /// Persists the provided <see cref="DbFeature"/> instances.
        /// </summary>
        /// <param name="features">A collection of <see cref="DbFeature"/> instances that should be persisted.</param>
        public async Task<IEnumerable<DbFeature>> GetAllDbFeatures()
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var result = await session.Query<DbFeature>().ToListAsync();
            _logger.LogInformation("Retrieved {FeatureCount} features", result.Count);
            return result;
        }

        /// <summary>
        /// Persists the provided <see cref="DbFeature"/> instances.
        /// </summary>
        /// <param name="features">A collection of <see cref="DbFeature"/> instances that should be persisted.</param>
        public async Task PersistDbFeatures(IEnumerable<DbFeature> features)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            foreach (var feature in features)
            {
                await session.StoreAsync(feature, feature.GetIdentifier());
            }

            await session.SaveChangesAsync();
        }

        private void AddChildren(FeatureDescription feature, Dictionary<string, List<FeatureDescription>> childRepository)
        {
            var strippedTitle = feature.Title.Replace(" ", string.Empty);

            if (childRepository.ContainsKey(strippedTitle))
            {
                feature.ChildFeatures = childRepository[strippedTitle];
                childRepository[strippedTitle].ForEach(f => AddChildren(f, childRepository));
            }
        }

        public async Task<DbFeature> InsertOrUpdateFeatureAsync(Feature feature, string productName, string groupName, string version)
        {
            _logger.LogInformation("Persisting feature {FeatureTitle} version {Version} for product {ProductName} and group {GroupName}",
                feature.Title, version, productName, groupName);

            var hash = feature.CalculateHash();

            DbFeature dbFeature;

            using (var session = _storeProvider.Store.OpenAsyncSession())
            {
                dbFeature = await session.LoadAsync<DbFeature>(DbFeatureExtensions.GetIdentifier(productName, groupName, feature.Title, hash));

                if (dbFeature != null)
                {
                    // Add the new version to the list
                    var versions = new List<string>(dbFeature.Versions) { version };

                    // Prevent duplicates
                    dbFeature.Versions = versions.Distinct().ToArray();
                }
                else
                {
                    // Create a new feature
                    var processor = new FeatureProcessor();
                    var parentTitle = processor.DetermineParent(feature);
                    dbFeature = new DbFeature(feature, productName, groupName, parentTitle, version) { Hash = hash };
                    await session.StoreAsync(dbFeature, dbFeature.GetIdentifier());
                }

                // Find any features with the same product name, group and title, but a different hash
                var wronglyStampedFeaturesQuery = session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                       .Where(f => f.Product == productName && f.Group == groupName && f.Title == feature.Title && f.Hash != hash);
                var wronglyStampedFeatures = await wronglyStampedFeaturesQuery.ToListAsync();

                // Check if any of those features have the current version number
                foreach (var wronglyStampedFeature in wronglyStampedFeatures.Where(f => f.Versions.Contains(version)))
                {
                    // If it is the only version for that feature
                    if (wronglyStampedFeature.Versions.Length == 1)
                    {
                        // Remove it entirely
                        session.Delete(wronglyStampedFeature);
                    }
                    else
                    {
                        // Otherwise remove the version number from that feature since it no longer matches the current feature
                        wronglyStampedFeature.Versions = wronglyStampedFeature.Versions.Where(v => v != version).ToArray();
                    }
                }

                await session.SaveChangesAsync();
            }

            return dbFeature;
        }

        public async Task PersistFeatureTestResultAsync(FeatureTestResult testResult, string productName, string groupName, string version)
        {
            // TODO Save testresults per version
            using var session = _storeProvider.Store.OpenAsyncSession();
            var dbFeature = await session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                     .Where(f => f.Product == productName
                                              && f.Group == groupName
                                              && f.Title == testResult.FeatureTitle
                                              && f.Version == version)
                                     .OfType<DbFeature>()
                                     .SingleOrDefaultAsync();
            if (dbFeature == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                              "Feature {0} does not exist for product {1} under group {2}.",
                              testResult.FeatureTitle,
                              productName,
                              groupName));
            }

            dbFeature.TestResult = testResult;

            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes all features in a specified group of the specified product.
        /// </summary>
        /// <param name="productName">The product under which he provided group falls.</param>
        /// <param name="groupName">The group of which the features should be deleted.</param>
        public async Task DeleteFeaturesAsync(string productName, string groupName)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            await session.Advanced.DocumentStore.Operations.Send(
                new DeleteByQueryOperation<DbFeature, Features_ByTitleProductAndGroup>(x =>
                    x.Product == productName && x.Group == groupName))
                .WaitForCompletionAsync();
        }

        /// <summary>
        /// Deletes all features of specified version in a specified group of the specified product.
        /// </summary>
        /// <param name="productName">The product under which he provided group falls.</param>
        /// <param name="groupName">The group of which the features should be deleted.</param>
        /// <param name="version">The version of the features to delete.</param>
        public async Task DeleteFeaturesAsync(string productName, string groupName, string version)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var dbFeatures = await session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                          .Where(f => f.Product == productName
                                                   && f.Group == groupName
                                                   && f.Version == version)
                                           .OfType<DbFeature>()
                                           .ToListAsync();

            foreach (var dbFeature in dbFeatures)
            {
                if (dbFeature.Versions.Length == 1)
                {
                    // Remove the feature as this is the only version
                    session.Delete(dbFeature);
                }
                else
                {
                    // Remove this version, but let the feature remain as it contains other versions
                    var versions = dbFeature.Versions.ToList();
                    versions.Remove(version);
                    dbFeature.Versions = versions.ToArray();
                }
            }

            await session.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes al versions of the specified feature.
        /// </summary>
        /// <param name="productName">The product the feature falls under.</param>
        /// <param name="groupName">The group the feature falls under.</param>
        /// <param name="title">The feature that should be deleted.</param>
        public async Task DeleteFeatureAsync(string productName, string groupName, string title)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            await session.Advanced.DocumentStore.Operations.Send(
                new DeleteByQueryOperation<DbFeature, Features_ByTitleProductAndGroup>(x =>
                    x.Product == productName && x.Group == groupName && x.Title == title))
                .WaitForCompletionAsync();
        }

        /// <summary>
        /// Deletes the specified version of the specified feature.
        /// </summary>
        /// <param name="productName">The product the feature falls under.</param>
        /// <param name="groupName">The group the feature falls under.</param>
        /// <param name="title">The feature that should be deleted.</param>
        /// <param name="version">The version of the feature that should be deleted.</param>
        public async Task DeleteFeatureAsync(string productName, string groupName, string title, string version)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var dbFeature = await session.Query<Features_ByTitleProductAndGroup.QueryModel, Features_ByTitleProductAndGroup>()
                                         .Where(f => f.Product == productName
                                                 && f.Group == groupName
                                                 && f.Title == title
                                                  && f.Version == version)
                                          .OfType<DbFeature>()
                                          .SingleOrDefaultAsync();
            if (dbFeature != null)
            {
                if (dbFeature.Versions.Length == 1)
                {
                    // Remove the feature as this is the only version
                    session.Delete(dbFeature);
                }
                else
                {
                    // Remove this version, but let the feature remain as it contains other versions
                    var versions = dbFeature.Versions.ToList();
                    versions.Remove(version);
                    dbFeature.Versions = versions.ToArray();
                }

                await session.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Searches for features which match te specified query (e.g. contain the content)
        /// </summary>
        /// <param name="query"></param>
        /// <returns>An enumerable of <see cref="FeatureMatch"/> instances containing the matches.</returns>
        public async Task<IEnumerable<FeatureMatch>> Search(string query)
        {
            using var session = _storeProvider.Store.OpenAsyncSession();
            var featureQuery = session.Query<DbFeature>()
                                      .Search(feature => feature.Description, query, 5)
                                      .Search(feature => feature.Scenarios, query, 5)
                                      .Search(feature => feature.Background, query, 5)
                                      .Search(feature => feature.Tags, query, 9)
                                      .Search(feature => feature.Title, query, 10);

            var queryResults = await featureQuery.ToListAsync();

            var comparer = new SemanticVersionComparer();

            var filteredResultsQuery = queryResults.Where(feature =>
                !queryResults.Any(feature2 => feature2.Title == feature.Title
                                           && feature2.Product == feature.Product
                                           && feature2.Hash != feature.Hash
                                           && comparer.Compare(feature2.Versions.OrderByDescending(v => v, comparer).First(),
                                                               feature.Versions.OrderByDescending(v => v, comparer).First()) > 0)
            );

            return filteredResultsQuery.Select(feature => feature.CreateFeatureMatch(query)).ToList();
        }
    }
}
