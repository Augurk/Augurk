/*
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
using Augurk.Entities.Analysis;
using Raven.Client.Indexes;
using System.Linq;

namespace Augurk.Api.Indeces.Analysis
{
    /// <summary>
    /// This index creates a map from the product and version.
    /// </summary>
    public class AnalysisReports_ByProductAndVersion : AbstractIndexCreationTask<AnalysisReport>
    {
        public AnalysisReports_ByProductAndVersion()
        {
            Map = reports => from report in reports
                             let metadata = MetadataFor(report)
                              select new
                              {
                                  Product = metadata.Value<string>("Product"),
                                  report.Version
                              };
        }

        public class Entry
        {
            public string Product { get; set; }
            public string Version { get; set; }
        }
    }
}
