/*
 Copyright 2020, Augurk

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
    public class Features_WithoutHash : AbstractIndexCreationTask<DbFeature>
    {
        public Features_WithoutHash()
        {
            Map = features => from feature in features
                              where feature.Hash == null
                              select new
                              {
                                  feature.Title,
                                  feature.Product,
                                  feature.Group
                              };

            Reduce = results => from result in results
                                group result by new {result.Title, result.Product, result.Group} into groupedResult
                                select new
                                {
                                    groupedResult.Key.Title,
                                    groupedResult.Key.Product,
                                    groupedResult.Key.Group
                                };
        }
    }
}
