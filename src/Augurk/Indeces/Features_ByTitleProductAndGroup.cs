/*
 Copyright 2015, 2020 Augurk

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

using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Augurk.Api.Indeces
{
    /// <summary>
    /// This index creates a map from the title, product and group found on a feature.
    /// </summary>
    public class Features_ByTitleProductAndGroup : AbstractIndexCreationTask<DbFeature>
    {
        public Features_ByTitleProductAndGroup()
        {
            Map = features => from feature in features
                              from Version in feature.Versions
                              select new
                              {
                                  feature.Title,
                                  feature.Product,
                                  feature.Group,
                                  feature.Hash,
                                  Version
                              };

            // Store the fields that are used often in the application,
            // so raven can return this data directly from the index.
            Stores.AddDbFeatureStoredFields();
        }

        public class QueryModel
        {
            public string Title { get; set; }
            public string ParentTitle { get; set; }
            public string Product { get; set; }
            public string Group { get; set; }

            public string Hash { get; set; }
            public string Version { get; set; }
        }
    }
}
