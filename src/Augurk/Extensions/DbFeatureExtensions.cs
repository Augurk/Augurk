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
            // Calculate the hash if it doesn't have it, it might still be an old feature
            return GetIdentifier(feature.Product, feature.Group, feature.Title, feature.Hash ?? feature.CalculateHash());
        }

        public static string GetIdentifier(string product, string group, string title, string hash)
        {

            string combinedTitle = String.Join("/", product.ToLowerInvariant(), group.ToLowerInvariant(), title.ToLowerInvariant());

            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.Default.GetBytes(combinedTitle);
                var guid = new Guid(cryptoServiceProvider.ComputeHash(bytes));
                var id = String.Format(CultureInfo.InvariantCulture,
                                       "{0}/{1}",
                                       hash,
                                       guid);
                return id;
            }
        }

        public static FeatureMatch CreateFeatureMatch(this DbFeature feature, string searchTerm)
        {
            var match = new FeatureMatch()
                {
                    FeatureName = feature.Title,
                    ShortenedDescription = feature.Description?.Substring(0, Math.Min(feature.Description.Length, 100)) + "...",
                    ProductName = feature.Product,
                    GroupName = feature.Group,
                    Version = feature.Versions.OrderBy(v => v, new SemanticVersionComparer()).FirstOrDefault(),
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

        /// <summary>
        /// Searches for the provided searchTerms and returns the first match for each term in a single <see cref="String"/>.
        /// </summary>
        /// <param name="source">The source string to search within.</param>
        /// <param name="searchTerms">The terms to search for.</param>
        /// <returns>A <see cref="String"/> containing the matching text, or an empty <see cref="String"/> if no match could be found.</returns>
        private static string FindMatchingText(string source, string[] searchTerms)
        {
            // Define the number of characters before and after the match that should be copied.
            const int characterPadding = 40;

            // Find al terms and determine which substrings we should take
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

            // Combine the substrings
            StringBuilder sb = new StringBuilder();
            int? cursor = null;
            foreach(var result in results.OrderBy(r => r.start).ThenBy(r => r.length))
            {
                // If the result overlaps with the previously copied result, adjust it so the fit seamlessly
                if(cursor.HasValue && result.start < cursor){
                    int lengthToRemove = cursor.Value - result.start;
                    sb.Remove(sb.Length - lengthToRemove, lengthToRemove);
                }
                // If the result does not fit seamlessly, add some ellipses
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
