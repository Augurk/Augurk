// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Augurk.Entities
{
    /// <summary>
    /// Describes a Feature.
    /// </summary>
    public class FeatureDescription
    {
        /// <summary>
        /// Gets or sets the title of this feature.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the latest version available of this feature.
        /// </summary>
        public string LatestVersion { get; set; }

        /// <summary>
        /// Gets or sets an enumerable collection of the descriptions of the child features of this feature.
        /// </summary>
        public IEnumerable<FeatureDescription> ChildFeatures { get; set; }
    }
}
