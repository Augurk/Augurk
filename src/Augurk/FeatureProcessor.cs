/*
 Copyright 2014-2016, Mark Taling
 
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
using Augurk.Entities;

namespace Augurk
{
    /// <summary>
    /// Contains logic to process all server tags on a feature.
    /// </summary>
    public class FeatureProcessor
    {
        private static readonly List<string> SERVER_TAGS = new List<string>() { "childOf", "parent", "notImplemented", "ignore" };
        private readonly List<string> _notImplementedTags = new List<string>() { "notImplemented" };
        private readonly List<string> _ignoreTags = new List<string>() { "ignore" };
        private readonly List<string> _childTags = new List<string>() { "childOf", "parent" };

        /// <summary>
        /// Processes the server tags on the provided feature.
        /// </summary>
        /// <param name="feature">The <see cref="DisplayableFeature"/> that should be processed.</param>
        public void Process(DisplayableFeature feature)
        {
            // Get the server tags
            IEnumerable<string> serverTags = GetServerTags(feature.Tags);

            foreach (var tag in serverTags)
            {
                string[] tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);

                    if (_notImplementedTags.Contains(tagParts[0]))
                    {
                        feature.Properties |= FeatureProperties.NotImplemented;
                    }
                    if (_ignoreTags.Contains(tagParts[0]))
                    {
                        feature.Properties |= FeatureProperties.Ignore;
                    }
            }

            // Remove all server tags
            RemoveServerTags(feature);
        }

        /// <summary>
        /// Gets the parent of the provided feature by parsing the tags.
        /// </summary>
        /// <param name="feature">The <see cref="Feature"/> for which the parent should be determined.</param>
        /// <returns>A <see cref="string"/> containing the title of the feature's parent, or null if the feature has no parent.</returns>
        public string DetermineParent(Feature feature)
        {
            foreach (var tag in feature.Tags)
            {
                string[] tagParts = tag.Split(new[] {":"}, 2, StringSplitOptions.RemoveEmptyEntries);
                if (_childTags.Contains(tagParts[0]))
                {
                    return tagParts[1];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a new collection containing all servertags that were present in the original collection.
        /// </summary>
        private static IEnumerable<string> GetServerTags(IEnumerable<string> tags)
        {
            List<string> filteredTags = new List<string>();

            foreach (var tag in tags)
            {
                string[] tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (SERVER_TAGS.Contains(tagParts[0]))
                {
                    filteredTags.Add(tag);
                }
            }

            return filteredTags;
        }

        /// <summary>
        /// Removes all tags that are recognized as server tags from the provided <see cref="Feature"/>
        /// and all underlying <see cref="Scenario"/>s.
        /// </summary>
        /// <param name="feature">The <see cref="Feature"/> instance </param>
        public static void RemoveServerTags(Feature feature)
        {
            if (feature.Tags != null)
            {
                feature.Tags = RemoveServerTags(feature.Tags);
            }
        }

        /// <summary>
        /// Returns a new collection containing the provided tags excluding any servertags.
        /// </summary>
        public static IEnumerable<string> RemoveServerTags(IEnumerable<string> tags)
        {
            List<string> filteredTags = new List<string>();

            foreach (var tag in tags)
            {
                string[] tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (!SERVER_TAGS.Contains(tagParts[0]))
                {
                    filteredTags.Add(tag);
                }
            }

            return filteredTags;
        }
    }
}