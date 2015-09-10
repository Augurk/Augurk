/*
 Copyright 2014-2015, Mark Taling
 
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Linq;
using Augurk.Api.Indeces;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods for retrieving products from storage.
    /// </summary>
    public class ProductsManager
    {
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                var products = await session.Query<DbFeature, Features_ByTitleProductAndGroup>().ToListAsync();
                return products.GroupBy(feature => feature.Product)
                               .Select(group => new Product
                               {
                                   Name = group.Key,
                                   GroupNames = group.Select(feature => feature.Group).ToList()
                               });
            }
        }
    }
}
