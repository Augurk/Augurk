// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Entities.Analysis
{
    /// <summary>
    /// Represents the different invocation kinds supported by Augurk
    /// </summary>
    public enum InvocationKind
    {
        /// <summary>
        /// Default value
        /// </summary>
        None = 0,

        /// <summary>
        /// The invocation represents an automated When step
        /// </summary>
        When = 1,

        /// <summary>
        /// The method invoked is marked as public
        /// </summary>
        Public = 2,

        /// <summary>
        /// The method invoked is marked as private
        /// </summary>
        Private = 4,

        /// <summary>
        /// The method invoked is marked as internal
        /// </summary>
        Internal = 8
    }
}
