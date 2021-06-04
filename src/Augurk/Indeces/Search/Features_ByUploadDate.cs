// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using Augurk.Entities;
using Raven.Client.Documents.Indexes;

namespace Augurk.Api.Indeces.Search
{
    ///<summary>
    /// This index creates a map from the upload date of a feature.
    ///</summary>
    public class Features_ByUploadDate : AbstractIndexCreationTask<DbFeature>
    {
        public Features_ByUploadDate()
        {
            Map = features => from feature in features
                              select new
                              {
                                  UploadDate = MetadataFor(feature).Value<DateTime>("upload-date"),
                                  feature.Scenarios,
                                  feature.Background,
                                  feature.Tags,
                                  feature.Title,
                                  feature.Description,
                                  feature.Product,
                                  feature.Group
                              };
            Index(f => f.Description, FieldIndexing.Search);
        }

        public class QueryModel
        {
            public DateTime UploadDate { get; set; }
            public IEnumerable<Scenario> Scenarios { get; set; }

            public Background Background { get; set; }

            public IEnumerable<string> Tags { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public string Product { get; set; }

            public string Group { get; set; }
        }
    }
}
