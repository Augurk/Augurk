/*
 Copyright 2015, Mark Taling
 
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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Raven.Abstractions.Indexing;

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
            // Store the branch so it can be used to create the top menu
            stores.Add(feature => feature.Branch, FieldStorage.Yes);

            // Store the three fields that are used to create the menu
            stores.Add(feature => feature.Group, FieldStorage.Yes);
            stores.Add(feature => feature.Title, FieldStorage.Yes);
            stores.Add(feature => feature.ParentTitle, FieldStorage.Yes);
        }
    }
}
