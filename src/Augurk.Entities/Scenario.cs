// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a feature scenario
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// Gets or sets the title of this scenario.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of this scenario.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags of this scenario.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the steps of this scenario.
        /// </summary>
        public IEnumerable<Step> Steps { get; set; }

        /// <summary>
        /// Gets or sets the example sets for this scenario.
        /// </summary>
        public IEnumerable<ExampleSet> ExampleSets { get; set; }

        /// <summary>
        /// Creates a textual representation of this background.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Scenario"/> instance.</returns>
        public override string ToString()
        {
            if (Steps == null || !Steps.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.AppendLine(Description);
            foreach (var step in Steps)
            {
                sb.AppendLine(step.ToString());
            }

            return sb.ToString();
        }
    }
}
