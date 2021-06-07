// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Entities
{
    /// <summary>
    /// Represents the Gherkin keywords
    /// </summary>
    public enum StepKeyword
    {
        /// <summary>
        /// No keyword.
        /// </summary>
        None,
        /// <summary>
        /// The Given keyword.
        /// </summary>
        Given,
        /// <summary>
        /// The Then keyword.
        /// </summary>
        Then,
        /// <summary>
        /// The When keyword.
        /// </summary>
        When,
        /// <summary>
        /// The And keyword.
        /// Usage of this keyword masks the actual meaning, check the <see cref="BlockKeyword"/> for that.
        /// </summary>
        And
    }
}
