/*
 Copyright 2015, 2020, Augurk

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
using Augurk.Entities.Search;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Augurk.Api
{
    public static class DbFeatureExtensions
    {
        public static string GetIdentifier(this DbFeature feature)
        {
            return GetIdentifier(feature.Product, feature.Group, feature.Title, feature.Version);
        }

        public static string GetIdentifier(string product, string group, string title, string version)
        {

            string combinedTitle = String.Join("/", product.ToLowerInvariant(), group.ToLowerInvariant(), title.ToLowerInvariant());

            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.Default.GetBytes(combinedTitle);
                var guid = new Guid(cryptoServiceProvider.ComputeHash(bytes));
                var id = String.Format(CultureInfo.InvariantCulture,
                                       "{0}/{1}",
                                       version.ToLowerInvariant(),
                                       guid);
                return id;
            }
        }

        public static FeatureMatch CreateFeatureMatch(this DbFeature feature, string searchTerm)
        {
            var match = new FeatureMatch()
                {
                    FeatureName = feature.Title,
                    ProductName = feature.Product,
                    GroupName = feature.Group,
                    Version = feature.Version,
                    Tags = feature.Tags.ToList()
                };

            string[] searchTerms = searchTerm.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            string matchingText = FindMatchingText(feature.Description, searchTerms);
            if(String.IsNullOrEmpty(matchingText) && feature.Background != null)
            {
                matchingText = FindMatchingText(feature.Background.ToString(), searchTerms);
            }
            var enumerator = feature.Scenarios.GetEnumerator();
            while(String.IsNullOrEmpty(matchingText) && enumerator.MoveNext())
            {
                matchingText = FindMatchingText(enumerator.Current.ToString(), searchTerms);
            }

            match.MatchingText = matchingText;

            return match;
        }

        private static string FindMatchingText(string source, string[] searchTerms)
        {
            const int characterPadding = 40;

            var results = new List<(int start, int length)>();
            if(searchTerms.Any(source.Contains))
            {
                foreach (string term in searchTerms)
                {
                    int index = source.IndexOf(term);
                    if(index >= 0)
                    {
                        int start = Math.Max(index - characterPadding, 0);
                        int length = Math.Min((index - start) + term.Length + characterPadding, source.Length - start);
                        results.Add((start, length));
                    }
                }
            }

            if(!results.Any())
            {
                return String.Empty;
            }

            StringBuilder sb = new StringBuilder();
            int? cursor = null;
            foreach(var result in results.OrderBy(r => r.start).ThenBy(r => r.length))
            {
                if(cursor.HasValue && result.start < cursor){
                    int lengthToRemove = cursor.Value - result.start;
                    sb.Remove(sb.Length - lengthToRemove, lengthToRemove);
                }
                else if(result.start > 0){
                    sb.Append("...");
                }

                sb.Append(source.Substring(result.start, result.length));

                cursor = result.start + result.length;
            }

            sb.Append("...");
            return sb.ToString();
        }
    }
}
