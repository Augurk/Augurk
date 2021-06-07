// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;

namespace Augurk.Entities.Test
{
    /// <summary>
    /// Represents the test result of a single scenario (or variant of a scenario).
    /// </summary>
    public class ScenarioTestResult
    {
        /// <summary>
        /// Gets or sets the title of the scenario.
        /// </summary>
        public string ScenarioTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the variant that was tested.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// Gets or sets the result of the test.
        /// </summary>
        public TestResult Result { get; set; }

        /// <summary>
        /// Gets or sets the date and time on which this test has been executed.
        /// </summary>
        public DateTime? TestExecutionDate { get; set; }
    }
}
