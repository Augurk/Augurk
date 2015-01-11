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

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Augurk.Api.Indeces;
using Raven.Client;
using Raven.Client.Linq;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to retrieve branches from storage.
    /// </summary>
    public class BranchManager
    {
        /// <summary>
        /// Gets the names of all available branches.
        /// </summary>
        /// <returns>An enumerable collection of branch names representend as a <see cref="string"/>.</returns>
        public async Task<IEnumerable<string>> GetBranchesAsync()
        {

            using (var session = Database.DocumentStore.OpenAsyncSession())
            {
                return await session.Query<DbFeature, Features_ByTitleBranchAndGroup>()
                                    .Select(feature => feature.Branch)
                                    .Distinct()
                                    .ToListAsync();
            }
        }
    }
}