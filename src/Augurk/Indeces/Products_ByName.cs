// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Raven.Client.Documents.Indexes;
using System.Linq;

namespace Augurk.Api.Indeces
{
    /// <summary>
    /// This index creates a map from the namne found on a product.
    /// </summary>
    public class Products_ByName : AbstractIndexCreationTask<DbProduct>
    {
        public Products_ByName()
        {
            Map = products => from product in products
                              select new { product.Name };
        }
    }
}
