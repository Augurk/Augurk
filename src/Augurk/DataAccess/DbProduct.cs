// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Api
{
    /// <summary>
    /// An implementation of a product designed specifically for storage in the database.
    /// </summary>
    public class DbProduct
    {
        /// <summary>
        /// Gets or sets the name of this product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this product in markdown syntax.
        /// </summary>
        public string DescriptionMarkdown { get; set; }
    }
}
