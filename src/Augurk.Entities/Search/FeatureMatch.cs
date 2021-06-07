// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Augurk.Entities.Search
{
    /// <summary>
    /// Represents a feature matching a search query.
    /// </summary>
    public class FeatureMatch
    {
        /// <summary>
        /// The name or title of the matching feature.
        /// </summary>
        public string FeatureName { get; set; }

        /// <summary>
        /// A shortened description of the feature which can be used for display purposes.
        /// </summary>
        public string ShortenedDescription { get; set; }

        /// <summary>
        /// The name of the product the feature falls under.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The name of the group the feature is placed in.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The name of the product the feature falls under.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The tags of the feature.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The text within the feature that matched the searchquery, if available.
        /// </summary>
        public string MatchingText { get; set; }
    }
}
