// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Augurk.Entities.Search
{
    /// <summary>
    /// A container for search results.
    /// </summary>
    public class SearchResults
    {
        /// <summary>
        /// Gets or sets the title of this feature.
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// Gets or sets a collection of features that match.
        /// </summary>
        public IEnumerable<FeatureMatch> FeatureMatches { get; set; }
    }
}
