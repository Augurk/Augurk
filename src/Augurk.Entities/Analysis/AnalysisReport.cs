// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;

namespace Augurk.Entities.Analysis
{
    public class AnalysisReport
    {
        /// <summary>
        /// The name of the analyzed project.
        /// </summary>
        /// <remarks>
        /// Augurk will use this name to display where the code for an automation was found.
        /// </remarks>
        public string AnalyzedProject { get; set; }

        /// <summary>
        /// The version of the code that was analyzed.
        /// </summary>
        /// <remarks>
        /// This field will be overwritten by the value provided during upload.
        /// </remarks>
        public string Version { get; set; }

        /// <summary>
        /// Indicates when this analysis was run.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The <see cref="Invocation">invocations</see> that were discovered during analysis.
        /// </summary>
        public Invocation[] RootInvocations { get; set; }
    }
}
