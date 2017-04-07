/*
 Copyright 2017, Augurk
 
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

using Raven.Client.Indexes;
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
