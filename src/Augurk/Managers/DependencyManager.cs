﻿// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Entities;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to query featuredependencies from Augurk.
    /// </summary>
    public class DependencyManager : IDependencyManager
    {
        private readonly IFeatureManager _featureManager;
        private readonly IAnalysisReportManager _analysisReportManager;
        private readonly ILogger<DependencyManager> _logger;

        public DependencyManager(IFeatureManager featureManager, IAnalysisReportManager analysisReportManager, ILogger<DependencyManager> logger)
        {
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
            _analysisReportManager = analysisReportManager ?? throw new ArgumentNullException(nameof(analysisReportManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the top level feature graphs, that is the graphs
        /// for features that do not have a parent feature.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="FeatureGraph"/> instances representing
        /// the dependency graphs for the unparented features.
        /// </returns>
        public async Task<IEnumerable<FeatureGraph>> GetTopLevelFeatureGraphsAsync()
        {
            var features = (await _featureManager.GetAllDbFeatures()).ToList();
            var invocations = (await _analysisReportManager.GetAllDbInvocations()).ToList();

            _logger.LogInformation("Calculating top level feature graphs for {FeatureCount} features and {InvocationCount} invocations.",
                features.Count, invocations.Count);

            var invocationFeatures = (from invocation in invocations
                                      select new
                                      {
                                          Invocation = invocation,
                                          Features = features.Where(f => f.DirectInvocationSignatures != null && f.DirectInvocationSignatures.Intersect(invocation.InvokedSignatures).Any()).ToList()
                                      }).ToList();

            var featuresInvocations = (from feature in features
                                       select new
                                       {
                                           Feature = feature,
                                           Invocations = invocationFeatures.Where(i => feature.DirectInvocationSignatures != null && feature.DirectInvocationSignatures.Contains(i.Invocation.Signature))
                                       }).ToList();

            var featureGraphs = new Dictionary<DbFeature, FeatureGraph>();
            foreach (var feature in featuresInvocations)
            {
                var featureGraph = new FeatureGraph
                {
                    FeatureName = feature.Feature.Title,
                    ProductName = feature.Feature.Product,
                    GroupName = feature.Feature.Group,
                    Tags = feature.Feature.Tags,
                    Version = feature.Feature.Version,
                    DependsOn = new List<FeatureGraph>()
                };

                featureGraphs.Add(feature.Feature, featureGraph);
            }

            var unparentedGraphs = new List<FeatureGraph>(featureGraphs.Values);
            foreach (var feature in featuresInvocations)
            {
                var featureGraph = featureGraphs[feature.Feature];
                var dependencies = feature.Invocations.SelectMany(i => i.Features).Select(f => featureGraphs[f]).ToList();
                dependencies.ForEach(f => unparentedGraphs.Remove(f));
                featureGraph.DependsOn.AddRange(dependencies.Distinct());
            }

            return unparentedGraphs;
        }

        public async Task<FeatureGraph> GetFeatureGraphAsync(string productName, string featureName, string version)
        {
            // Create the result variable
            var result = new FeatureGraph
            {
                ProductName = productName,
                FeatureName = featureName,
                Version = version
            };

            // For now, just use all tree as a source
            var trees = (await GetTopLevelFeatureGraphsAsync()).ToList();

            trees.ForEach(tree => ExtractFeatureRelations(tree, result));

            return result;
        }

        private void ExtractFeatureRelations(FeatureGraph node, FeatureGraph result)
        {
            if (node.DescribesSameFeature(result))
            {
                // If it is the first time we encounter ourselves, continue on
                if (result.DependsOn.Count == 0)
                {
                    foreach (var dependency in node.DependsOn)
                    {
                        result.DependsOn.Add(new FeatureGraph()
                        {
                            ProductName = dependency.ProductName,
                            FeatureName = dependency.FeatureName,
                            GroupName = dependency.GroupName,
                            Version = dependency.Version
                        });
                    }
                }
                // otherwise, return
                else
                {
                    return;
                }
            }

            if (node.DependsOn.Any(dependency => dependency.DescribesSameFeature(result))
               && !result.Dependants.Any(dependant => dependant.DescribesSameFeature(node)))
            {
                result.Dependants.Add(new FeatureGraph()
                {
                    ProductName = node.ProductName,
                    FeatureName = node.FeatureName,
                    GroupName = node.GroupName,
                    Version = node.Version
                });
            }

            node.DependsOn.ForEach(dependency => ExtractFeatureRelations(dependency, result));
        }
    }
}
