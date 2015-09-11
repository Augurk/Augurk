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

using Augurk.Api.Managers;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Augurk.Api
{
    public static class DbFeatureExtensions
    {
        public static string GetIdentifier(this DbFeature feature)
        {
            return GetIdentifier(feature.Product, feature.Group, feature.Title, feature.Branch ?? "unknown");
        }

        public static string GetIdentifier(string product, string branch, string group, string title)
        {

            string combinedTitle = String.Join("/", product.ToLowerInvariant(), group.ToLowerInvariant(), title.ToLowerInvariant(), branch.ToLowerInvariant());

            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.Default.GetBytes(combinedTitle);
                var guid = new Guid(cryptoServiceProvider.ComputeHash(bytes));
                var id = String.Format(CultureInfo.InvariantCulture,
                                       "{0}/{1}",
                                       branch.ToLowerInvariant(),
                                       guid);
                return id;
            }
        }
    }
}
