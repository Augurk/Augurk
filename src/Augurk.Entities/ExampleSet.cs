// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents an example set
    /// </summary>
    public class ExampleSet : Table
    {
        /// <summary>
        /// Gets or sets the title of this example set.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of this example set.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the keyword for this example set.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the tags of this example set.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
