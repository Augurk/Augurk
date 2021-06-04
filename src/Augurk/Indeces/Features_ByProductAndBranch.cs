// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Raven.Client.Documents.Indexes;

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
                              select new
                              {
                                  Tag = tag,
                                  feature.Title,
                                  feature.Product,
                                  feature.Group
                              };

            StoreAllFields(FieldStorage.Yes);
        }

        public class TaggedFeature
        {
            public string Tag { get; set; }
            public string Product { get; set; }
            public string Title { get; set; }
            public string Group { get; set; }
            public string ParentTitle { get; set; }
        }
    }
}
