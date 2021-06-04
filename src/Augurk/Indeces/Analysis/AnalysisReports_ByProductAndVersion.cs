// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Entities.Analysis;
using Raven.Client.Documents.Indexes;
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
