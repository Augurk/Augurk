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

namespace Augurk.Api.Managers
{
    public interface IDependencyManager
    {
        /// <summary>
        /// Gets the top level feature graphs, that is the graphs 
        /// for features that do not have a parent feature.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="FeatureGraph"/> instances representing 
        /// the dependency graphs for the unparented features.
        /// </returns>
        Task<IEnumerable<FeatureGraph>> GetTopLevelFeatureGraphsAsync();

        /// <summary>
        /// Gets the graph of features depending on the requested feature,
        /// as well as the features the requested feature depends on.
        /// </summary>
        /// <param name="productName">Name of the product containing the feature.</param>
        /// <param name="featureName">Name of the feature.</param>
        /// <param name="version">Version of the feature.</param>
        /// <returns>A <see cref="FeatureGraph" /> instance representing the
        /// dependency graph for the requested feature.</returns>
        Task<FeatureGraph> GetFeatureGraphAsync(string productName, string featureName, string version);
    }
}