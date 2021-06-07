// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents the different properties of a feature
    /// </summary>
    [Flags]
    public enum FeatureProperties
    {
        /// <summary>
        /// This feature has no properties
        /// </summary>
        None = 0,

        /// <summary>
        /// This feature should be ignored by the testengine
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// This feature has not been implemented
        /// </summary>
        NotImplemented = 2,
    }
}
