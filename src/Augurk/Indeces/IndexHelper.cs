// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Raven.Client.Documents.Indexes;

namespace Augurk.Api.Indeces
{
    /// <summary>
    /// A helper class that provides methods intended for usage in multiple indeces.
    /// </summary>
    public static class IndexHelper
    {
        /// <summary>
        /// Adds the fields of the <see cref="DbFeature"/> class that should be stored to the provided index stores.
        /// </summary>
        /// <param name="stores">The stores to which the fields should be added.</param>
        public static void AddDbFeatureStoredFields(this IDictionary<Expression<Func<DbFeature, object>>, FieldStorage> stores)
        {
            // Store the product so it can be used to create the top menu
            stores.Add(feature => feature.Product, FieldStorage.Yes);

            // Store the three fields that are used to create the menu
            stores.Add(feature => feature.Group, FieldStorage.Yes);
            stores.Add(feature => feature.Title, FieldStorage.Yes);
            stores.Add(feature => feature.ParentTitle, FieldStorage.Yes);
        }
    }
}
