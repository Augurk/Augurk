// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
                                group result by new { result.Title, result.Product, result.Group } into groupedResult
                                select new
                                {
                                    groupedResult.Key.Title,
                                    groupedResult.Key.Product,
                                    groupedResult.Key.Group
                                };
        }
    }
}
