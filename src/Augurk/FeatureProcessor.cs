// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
        private static readonly List<string> SERVER_TAGS = new() { "childOf", "parent", "notImplemented", "ignore" };
        private readonly List<string> _notImplementedTags = new() { "notImplemented" };
        private readonly List<string> _ignoreTags = new() { "ignore" };
        private readonly List<string> _childTags = new() { "childOf", "parent" };

        /// <summary>
        /// Processes the server tags on the provided feature.
        /// </summary>
        /// <param name="feature">The <see cref="DisplayableFeature"/> that should be processed.</param>
        public void Process(DisplayableFeature feature)
        {
            // Get the server tags
            var serverTags = GetServerTags(feature.Tags);

            foreach (var tag in serverTags)
            {
                var tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);

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
                var tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
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
            var filteredTags = new List<string>();

            foreach (var tag in tags)
            {
                var tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
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
            var filteredTags = new List<string>();

            foreach (var tag in tags)
            {
                var tagParts = tag.Split(new[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (!SERVER_TAGS.Contains(tagParts[0]))
                {
                    filteredTags.Add(tag);
                }
            }

            return filteredTags;
        }
    }
}
