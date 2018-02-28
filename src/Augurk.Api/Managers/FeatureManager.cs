/*
 Copyright 2014-2017, Augurk
 
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api.Indeces;
using Augurk.Entities;
using Augurk.Entities.Test;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Raven.Client;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using Group = Augurk.Entities.Group;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve features from storage.
    /// </summary>
    public class FeatureManager
    {
        /// <summary>
        /// Gets or sets the JsonSerializerSettings that should be used when (de)serializing.
        /// </summary>
        internal static JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Gets or sets the configuration manager which should be used by this instance.
        /// </summary>
        private ConfigurationManager ConfigurationManager { get; set; }

        public FeatureManager()
            : this(new ConfigurationManager())
        {
               
        }

        internal FeatureManager(ConfigurationManager configurationManager)
        {
            ConfigurationManager = configurationManager;
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
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var versions = await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                            .Where(feature => feature.Product == productName && feature.Group == groupName && feature.Title == title)
                                            .Select(feature => feature.Version)
                                            .ToListAsync();

                return versions.OrderByDescending(version => version, new SemanticVersionComparer());
            }
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
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var dbFeature = await session.LoadAsync<DbFeature>(DbFeatureExtensions.GetIdentifier(productName, groupName, title, version));

                if (dbFeature == null)
                {
                    return null;
                }

                var result = new DisplayableFeature(dbFeature);
                result.TestResult = dbFeature.TestResult;
                result.Version = dbFeature.Version;

                // Process the server tags
                var processor = new FeatureProcessor();
                processor.Process(result);

                return result;
            }
        }

        /// <summary>
        /// Gets groups containing the descriptions for all features for the specified branch.
        /// </summary>
        /// <param name="productName">The name of the product for which the feature descriptions should be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="Entities.Group"/> instances.</returns>
        public async Task<IEnumerable<Group>> GetGroupedFeatureDescriptionsAsync(string productName)
        {
            Dictionary<string, List<FeatureDescription>> featureDescriptions = new Dictionary<string, List<FeatureDescription>>();
            Dictionary<string, Group> groups = new Dictionary<string, Group>();

            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var data = await session.Query<DbFeature, Features_ByTitleProductAndGroup>()
                                        .Where(feature => feature.Product.Equals(productName, StringComparison.OrdinalIgnoreCase))
                                        .Select(feature =>
                                                new
                                                {
                                                    feature.Group,
                                                    feature.ParentTitle,
                                                    feature.Title,
                                                    feature.Version
                                                })
                                        .Take(1000)
                                        .ToListAsync();

                foreach (var uniqueFeature in data.GroupBy(record => record.Title))
                {
                    var latestFeature = uniqueFeature.OrderByDescending(record => record.Version, new SemanticVersionComparer()).First();
                    var featureDescription = new FeatureDescription()
                    {
                        Title = uniqueFeature.Key,
                        LatestVersion = latestFeature.Version,
                    };

                    if (String.IsNullOrWhiteSpace(latestFeature.ParentTitle))
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
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
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
        }

        /// <summary>
        /// Gets a collection of features for the specified <paramref name="productName">product</paramref> and <paramref name="groupName">group</paramref>.
        /// </summary>
        /// <param name="productName">The name of the branch for which the feature descriptions should be retrieved.</param>
        /// <param name="groupName">A tag which should be used to filter the results.</param>
        /// <returns>An enumerable collection of <see cref="FeatureDescription"/> instances.</returns>
        public async Task<IEnumerable<FeatureDescription>> GetFeatureDescriptionsByProductAndGroupAsync(string productName, string groupName)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
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
        }

        private void AddChildren(FeatureDescription feature, Dictionary<string, List<FeatureDescription>> childRepository)
        {
            var strippedTitle = feature.Title.Replace(" ", String.Empty);

            if (childRepository.ContainsKey(strippedTitle))
            {
                feature.ChildFeatures = childRepository[strippedTitle];
                childRepository[strippedTitle].ForEach(f => AddChildren(f, childRepository));
            }
        }

        public async Task InsertOrUpdateFeatureAsync(Feature feature, string productName, string groupName, string version)
        {
            var processor = new FeatureProcessor();
            string parentTitle = processor.DetermineParent(feature);

            DbFeature dbFeature = new DbFeature(feature, productName, groupName, parentTitle, version);

            var configuration = await ConfigurationManager.GetOrCreateConfigurationAsync();

            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                // Using the store method when the feature already exists in the database will override it completely, this is acceptable
                await session.StoreAsync(dbFeature, dbFeature.GetIdentifier());

                session.SetExpirationIfEnabled(dbFeature, version, configuration);

                await session.SaveChangesAsync();
            }
        }

        public async Task PersistFeatureTestResultAsync(FeatureTestResult testResult, string productName, string groupName, string version)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var dbFeature = await session.LoadAsync<DbFeature>(DbFeatureExtensions.GetIdentifier(productName, groupName, testResult.FeatureTitle, version));

                if (dbFeature == null)
                {
                    throw new Exception(String.Format(CultureInfo.InvariantCulture,
                                  "Feature {0} does not exist for product {1} under group {2}.",
                                  testResult.FeatureTitle,
                                  productName,
                                  groupName));
                }

                dbFeature.TestResult = testResult;

                await session.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes all features in a specified group of the specified product.
        /// </summary>
        /// <param name="productName">The product under which he provided group falls.</param>
        /// <param name="groupName">The group of which the features should be deleted.</param>
        public async Task DeleteFeaturesAsync(string productName, string groupName)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync(
                    nameof(Features_ByTitleProductAndGroup).Replace('_', '/'),
                    new IndexQuery() { Query = $"Product:\"{productName}\"AND Group:\"{groupName}\"" },
                    new BulkOperationOptions() { AllowStale = true });
            }
        }

        /// <summary>
        /// Deletes all features of specified version in a specified group of the specified product.
        /// </summary>
        /// <param name="productName">The product under which he provided group falls.</param>
        /// <param name="groupName">The group of which the features should be deleted.</param>
        /// <param name="version">The version of the features to delete.</param>
        public async Task DeleteFeaturesAsync(string productName, string groupName, string version)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync(
                    nameof(Features_ByTitleProductAndGroup).Replace('_', '/'),
                    new IndexQuery() { Query = $"Product:\"{productName}\"AND Group:\"{groupName}\"AND Version:\"{version}\"" },
                    new BulkOperationOptions() { AllowStale = true });
            }
        }

        /// <summary>
        /// Deletes al versions of the specified feature.
        /// </summary>
        /// <param name="productName">The product the feature falls under.</param>
        /// <param name="groupName">The group the feature falls under.</param>
        /// <param name="title">The feature that should be deleted.</param>
        public async Task DeleteFeatureAsync(string productName, string groupName, string title)
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                await session.Advanced.DocumentStore.AsyncDatabaseCommands.DeleteByIndexAsync(
                    nameof(Features_ByTitleProductAndGroup).Replace('_', '/'),
                    new IndexQuery() { Query = $"Product:\"{productName}\"AND Group:\"{groupName}\"AND Title:\"{title}\"" },
                    new BulkOperationOptions() { AllowStale = true });
            }
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
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                // The delete method only marks the entity with the provided id for deletion, as such it is not asynchronous
                session.Delete(DbFeatureExtensions.GetIdentifier(productName, groupName, title, version));

                await session.SaveChangesAsync();
            }
        }
    }
}