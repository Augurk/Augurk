/*
 Copyright 2015, Mark Taling
 
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

using System.Linq;
using Raven.Client.Indexes;

namespace Augurk.Api.Indeces
{
    ///<summary>
    /// This index creates a map from the tags and product found on a feature. For performance and memory reasons only the first 50 distinct tags will be indexed.
    ///</summary>
    public class Features_ByProductAndBranch : AbstractIndexCreationTask<DbFeature, Features_ByProductAndBranch.TaggedFeature>
    {
        public Features_ByProductAndBranch()
        {
            Map = features => from feature in features
                              from tag in
                                  feature.Tags
                                         .Union(feature.Scenarios.SelectMany(scenario => scenario.Tags))
                                         .Distinct()
                                         .Take(50)
                              select new
                                  {
                                      Tag = tag,
                                      feature.Title,
                                      feature.Product,
                                      feature.Group
                              };

            MaxIndexOutputsPerDocument = 50;

            StoreAllFields(Raven.Abstractions.Indexing.FieldStorage.Yes);
        }

        public class TaggedFeature
        {
            public string Tag { get; set;}
            public string Product { get; set; }
            public string Title { get; set; }
            public string Group { get; set; }
            public string ParentTitle { get; set; }
        }
    }
}
