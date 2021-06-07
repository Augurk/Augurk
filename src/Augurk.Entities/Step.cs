// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a single step within a scenario
    /// </summary>
    public class Step
    {
        /// <summary>
        /// Gets or sets the block keyword for this step.
        /// </summary>
        public BlockKeyword BlockKeyword { get; set; }

        /// <summary>
        /// Gets or sets the keyword for this step.
        /// </summary>
        public StepKeyword StepKeyword { get; set; }

        /// <summary>
        /// Gets or sets the localized keyword for this step.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the content of this step.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the table argument for this step.
        /// </summary>
        public Table TableArgument { get; set; }

        public override string ToString()
        {
            return $"{Keyword} {Content} {(TableArgument == null ? "" : Environment.NewLine)}{TableArgument?.ToString()}";
        }
    }
}
