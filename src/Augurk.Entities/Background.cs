﻿/*
 Copyright 2014, 2020 Augurk

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
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
        /// <returns>A <see cref="System.String"/> representation of this <see cref="Background"/> instance.</returns>
        public override string ToString()
        {
            if (Steps == null || !Steps.Any())
            {
                return String.Empty;
            }

            var sb = new StringBuilder();
            foreach(var step in Steps)
            {
                sb.AppendLine(step.ToString());
            }

            return sb.ToString();
        }
    }
}
