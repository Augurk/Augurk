/*
 Copyright 2014, 2020, Mark Taling

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
            sb.AppendLine(String.Join(" ", Tags));
            sb.AppendLine(Title);
            if(Background != null){
                sb.AppendLine(Background.ToString());
            }
            foreach(var scenario in Scenarios)
            {
                sb.AppendLine(scenario.ToString());
            }

            // Hash the string representation
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hashedData = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                var hashBuilder = new StringBuilder();
                for (int i = 0; i < hashedData.Length; i++)
                {
                    hashBuilder.Append(hashedData[i].ToString("x2"));
                }
                return hashBuilder.ToString();
            }
        }
    }
}