/*
 Copyright 2020, Augurk

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

namespace Augurk.Entities.Search
{
    /// <summary>
    /// Represents a feature matching a search query.
    /// </summary>
    public class FeatureMatch
    {
        /// <summary>
        /// The name or title of the matching feature.
        /// </summary>
        public string FeatureName { get; set; }

        /// <summary>
        /// A shortened description of the feature which can be used for display purposes.
        /// </summary>
        public string ShortenedDescription { get; set; }

        /// <summary>
        /// The name of the product the feature falls under.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// The name of the group the feature is placed in.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The name of the product the feature falls under.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The tags of the feature.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The text within the feature that matched the searchquery, if available.
        /// </summary>
        public string MatchingText { get; set; }
    }
}