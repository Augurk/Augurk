// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Text;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a table
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Gets or sets the columns for this table.
        /// </summary>
        public IEnumerable<string> Columns { get; set; }

        /// <summary>
        /// Gets or sets the rows for this table.
        /// </summary>
        public IEnumerable<IEnumerable<string>> Rows { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("|");
            sb.Append(string.Join("|", Columns));
            sb.Append("|");
            sb.Append(Environment.NewLine);

            foreach (var row in Rows)
            {
                sb.Append("|");
                sb.Append(string.Join("|", row));
                sb.Append("|");
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
