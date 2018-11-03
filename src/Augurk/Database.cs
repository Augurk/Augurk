/*
 Copyright 2015, Augurk
 
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
using Raven.Client;

namespace Augurk.Api
{
    /// <summary>
    /// A static accesspoint for the database
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// Gets or sets the documentstore which should be used to access the features.
        /// </summary>
        public static IDocumentStore DocumentStore { get; set; } 
    }
}
