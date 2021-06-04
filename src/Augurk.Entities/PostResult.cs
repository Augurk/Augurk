// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a rest result
    /// </summary>
    public class PostResult
    {
        /// <summary>
        /// The Uniform Resource Locator at which the API provides the posted content.
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// The Uniform Resource Locator at which the Augurk UI provides the posted content.
        /// </summary>
        public string WebUrl { get; set; }
    }
}
