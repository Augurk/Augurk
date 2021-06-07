// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
