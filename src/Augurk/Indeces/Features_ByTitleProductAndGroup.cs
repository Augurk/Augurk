// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
