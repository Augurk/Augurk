﻿/*
 Copyright 2018, Augurk
 
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
using Augurk.Entities;
using Raven.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to query featuredependencies from Augurk.
    /// </summary>
    public class DependencyManager
    {
        /// <summary>
        /// Gets the top level feature graphs, that is the graphs 
        /// for features that do not have a parent feature.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="FeatureGraph"/> instances representing 
        /// the dependency graphs for the unparented features.
        /// </returns>
        public async Task<IEnumerable<FeatureGraph>> GetTopLevelFeatureGraphs()
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var features = await session.Query<DbFeature>().ToListAsync();
                var invocations = await session.Query<DbInvocation>().ToListAsync();

                var invocationFeatures = from invocation in invocations
                                         select new
                                         {
                                             Invocation = invocation,
                                             Features = features.Where(f => f.DirectInvocationSignatures.Intersect(invocation.InvokedSignatures).Any()).ToList()
                                         };

                var featuresInvocations = from feature in features
                                          select new
                                          {
                                              Feature = feature,
                                              Invocations = invocationFeatures.Where(i => feature.DirectInvocationSignatures.Contains(i.Invocation.Signature))
                                          };

                var featureGraphs = new Dictionary<DbFeature, FeatureGraph>();
                foreach (var feature in featuresInvocations)
                {
                    var featureGraph = new FeatureGraph
                    {
                        FeatureName = feature.Feature.Title,
                        ProductName = feature.Feature.Product,
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
        }
    }
}
