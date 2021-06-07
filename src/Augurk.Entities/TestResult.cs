// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a rest result
    /// </summary>
    public enum TestResult
    {
        /// <summary>
        /// There is no testresult
        /// </summary>
        None = 0,

        /// <summary>
        /// The test has passed
        /// </summary>
        Passed = 1,

        /// <summary>
        /// The testresults are inconclusive
        /// </summary>
        Inconclusive = 2,

        /// <summary>
        /// The test has failed
        /// </summary>
        Failed = 3
    }
}
