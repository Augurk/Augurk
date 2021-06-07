// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a feature
    /// </summary>
    public class Feature
    {
        public Feature()
        {
            Tags = new List<string>();
            Scenarios = new List<Scenario>();
        }

        /// <summary>
        /// Gets or sets the title of the feature.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of this feature.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags of this feature.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the scenarios of this feature.
        /// </summary>
        public IEnumerable<Scenario> Scenarios { get; set; }

        /// <summary>
        /// Gets or sets the background of this feature.
        /// </summary>
        public Background Background { get; set; }

        /// <summary>
        /// Calculates a SHA256 hash for this feature.
        /// </summary>
        /// <returns>The SHA256 hash for this feature.</returns>
        public string CalculateHash()
        {
            // Create a string representation of this feature.
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(" ", Tags));
            sb.AppendLine(Title);
            sb.AppendLine(Description);
            if (Background != null)
            {
                sb.AppendLine(Background.ToString());
            }
            foreach (var scenario in Scenarios)
            {
                sb.AppendLine(scenario.ToString());
            }

            // Hash the string representation
            using (var sha256Hash = SHA256.Create())
            {
                var hashedData = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                var hashBuilder = new StringBuilder();
                for (var i = 0; i < hashedData.Length; i++)
                {
                    hashBuilder.Append(hashedData[i].ToString("x2"));
                }
                return hashBuilder.ToString();
            }
        }
    }
}
