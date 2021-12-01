// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a product
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the name of the feature, as used by the API.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the feature, as used for display purposes.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the short description of this product.
        /// This description can be used when listing multiple products.
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the description of this product.
        /// </summary>
        public string Description { get; set; }
    }
}
