// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a feature background
    /// </summary>
    public class Background
    {
        /// <summary>
        /// Gets or sets the title of this background.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the localized keyword for this background.
        /// </summary>
        public string Keyword { get; set; }


        /// <summary>
        /// Gets or sets the steps of this background.
        /// </summary>
        public IEnumerable<Step> Steps { get; set; }

        /// <summary>
        /// Creates a textual representation of this background.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="Background"/> instance.</returns>
        public override string ToString()
        {
            if (Steps == null || !Steps.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var step in Steps)
            {
                sb.AppendLine(step.ToString());
            }

            return sb.ToString();
        }
    }
}
