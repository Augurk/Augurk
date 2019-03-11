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
using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Entities;
using Augurk.Entities.Test;

namespace Augurk.Api.Managers
{
    public interface IFeatureManager
    {
        /// <summary>
        /// Gets the available versions of a particular feature.
        /// </summary>
        /// <param name="productName">Name of the product the feature belongs to.</param>
        /// <param name="groupName">Name of the group the product belongs to.</param>
        /// <param name="title">Title of the feature to get the versions for.</param>
        /// <returns>Returns a range of available versions.</returns>
        Task<IEnumerable<string>> GetFeatureAvailableVersions(string productName, string groupName, string title);

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
        Task<DisplayableFeature> GetFeatureAsync(string productName, string groupName, string title, string version);

        /// <summary>
        /// Gets groups containing the descriptions for all features for the specified branch.
        /// </summary>
        /// <param name="productName">The name of the product for which the feature descriptions should be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="Entities.Group"/> instances.</returns>
        Task<IEnumerable<Group>> GetGroupedFeatureDescriptionsAsync(string productName);

        /// <summary>
        /// Gets a collection of features for the specified <paramref name="branchName">branch</paramref> and tag.
        /// </summary>
        /// <param name="branchName">The name of the branch for which the feature descriptions should be retrieved.</param>
        /// <param name="tag">A tag which should be used to filter the results.</param>
        /// <returns>An enumerable collection of <see cref="FeatureDescription"/> instances.</returns>
        Task<IEnumerable<FeatureDescription>> GetFeatureDescriptionsByBranchAndTagAsync(string branchName, string tag);

        /// <summary>
        /// Gets a collection of features for the specified <paramref name="productName">product</paramref> and <paramref name="groupName">group</paramref>.
        /// </summary>
        /// <param name="productName">The name of the product for which the feature descriptions should be retrieved.</param>
        /// <param name="groupName">The group which should be used to filter the results.</param>
        /// <returns>An enumerable collection of <see cref="FeatureDescription"/> instances.</returns>
        Task<IEnumerable<FeatureDescription>> GetFeatureDescriptionsByProductAndGroupAsync(string productName, string groupName);

        /// <summary>
        /// Gets a collection of <see cref="DbFeature"/> instances that match the 
        /// provided <paramref name="productName"/> and <paramref name="version"/>.
        /// </summary>
        /// <param name="productName">The name of the product for which the features should be retrieved.</param>
        /// <param name="version">The version of the product for which the features should be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="DbFeature"/> instances.</returns>
        Task<IEnumerable<DbFeature>> GetDbFeaturesByProductAndVersionAsync(string productName, string version);

        /// <summary>
        /// Gets all the stored features from the database.
        /// </summary>
        /// <returns>A range of <see cref="DbFeature" /> instances representing the features available in the database.</returns>
        Task<IEnumerable<DbFeature>> GetAllDbFeatures();

        /// <summary>
        /// Persists the provided <see cref="DbFeature"/> instances.
        /// </summary>
        /// <param name="features">A collection of <see cref="DbFeature"/> instances that should be persisted.</param>
        Task PersistDbFeatures(IEnumerable<DbFeature> features);

        Task<DbFeature> InsertOrUpdateFeatureAsync(Feature feature, string productName, string groupName, string version);

        Task PersistFeatureTestResultAsync(FeatureTestResult testResult, string productName, string groupName, string version);

        /// <summary>
        /// Deletes all features in a specified group of the specified product.
        /// </summary>
        /// <param name="productName">The product under which he provided group falls.</param>
        /// <param name="groupName">The group of which the features should be deleted.</param>
        Task DeleteFeaturesAsync(string productName, string groupName);

        /// <summary>
        /// Deletes all features of specified version in a specified group of the specified product.
        /// </summary>
        /// <param name="productName">The product under which he provided group falls.</param>
        /// <param name="groupName">The group of which the features should be deleted.</param>
        /// <param name="version">The version of the features to delete.</param>
        Task DeleteFeaturesAsync(string productName, string groupName, string version);

        /// <summary>
        /// Deletes al versions of the specified feature.
        /// </summary>
        /// <param name="productName">The product the feature falls under.</param>
        /// <param name="groupName">The group the feature falls under.</param>
        /// <param name="title">The feature that should be deleted.</param>
        Task DeleteFeatureAsync(string productName, string groupName, string title);

        /// <summary>
        /// Deletes the specified version of the specified feature.
        /// </summary>
        /// <param name="productName">The product the feature falls under.</param>
        /// <param name="groupName">The group the feature falls under.</param>
        /// <param name="title">The feature that should be deleted.</param>
        /// <param name="version">The version of the feature that should be deleted.</param>
        Task DeleteFeatureAsync(string productName, string groupName, string title, string version);
    }
}