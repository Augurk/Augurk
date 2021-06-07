// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Indexes;

namespace Augurk.Api.Indeces.Analysis
{
    ///<summary>
    /// This index persists which direct invocations are made
    ///</summary>
    public class Features_ByDirectInvocations : AbstractIndexCreationTask<DbFeature, Features_ByDirectInvocations.Feature>
    {
        public Features_ByDirectInvocations()
        {
            Map = features => from feature in features
                              select new
                              {
                                  feature.Title,
                                  feature.Product,
                                  feature.Group,
                                  DirectInvocations = feature.DirectInvocationSignatures
                              };

            StoreAllFields(FieldStorage.Yes);
        }

        public class Feature
        {
            public string Title { get; set; }
            public string Product { get; set; }
            public string Version { get; set; }
            public IList<string> DirectInvocations { get; set; }
        }
    }
}
