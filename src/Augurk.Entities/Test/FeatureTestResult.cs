// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;

namespace Augurk.Entities.Test
{
    /// <summary>
    /// Represents the test result of a single feature.
    /// </summary>
    public class FeatureTestResult
    {
        /// <summary>
        /// Gets or sets the title of the feature.
        /// </summary>
        public string FeatureTitle { get; set; }

        /// <summary>
        /// Gets or sets the overal test result of the feature.
        /// </summary>
        public TestResult Result { get; set; }

        /// <summary>
        /// Gets or sets an enumerable collection of the test results of the scenario's that are part of the feature.
        /// </summary>
        public IEnumerable<ScenarioTestResult> ScenarioTestResults { get; set; }

        /// <summary>
        /// Gets or sets the date and time on which this test has been executed.
        /// </summary>
        public DateTime TestExecutionDate { get; set; }
    }
}
