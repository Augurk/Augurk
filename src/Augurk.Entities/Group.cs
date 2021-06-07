// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a group containing features.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Gets or sets the name of this group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets an enumerable collection containing the features in this group.
        /// </summary>
        public IEnumerable<FeatureDescription> Features { get; set; }
    }
}
