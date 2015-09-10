/*
 Copyright 2015, Mark Taling
 
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

using Augurk.Entities;
using Augurk.Entities.Test;
using System;

namespace Augurk.Api
{
    /// <summary>
    /// An implementation of <see cref="Feature"/> designed specifically for storage in the database.
    /// </summary>
    public class DbFeature : Feature
    {
        /// <summary>
        /// Gets or sets the branch this feature falls under.
        /// </summary>
        [Obsolete("Support for branches will be dropped in a future version.")]
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the title of the group this feature falls under.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the title of the parent feature.
        /// </summary>
        public string ParentTitle { get; set; }

        /// <summary>
        /// Gets or sets the the test result
        /// </summary>
        public FeatureTestResult TestResult { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFeature"/> class.
        /// </summary>
        public DbFeature()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbFeature"/> class using the provided <see cref="Feature"/> to provide initial values.
        /// </summary>
        /// <param name="feature">The <see cref="Feature"/>that should be used when determining the the initial values.</param>
        /// <param name="branch">The branch this feature falls under.</param>
        /// <param name="group">The group this feature falls under.</param>
        /// <param name="parentTitle">The title of the parent feature.</param>
        /// <remarks>This constructor does not wrap the provided feature, it will result in an actual copy with shared scenarios and a shared background.</remarks>
        public DbFeature(Feature feature, string branch, string group, string parentTitle)
        {
            Product = feature.Product;
            Branch = branch;
            Group = group;
            ParentTitle = parentTitle;

            // Copy the properties from the provided feature
            Title = feature.Title;
            Description = feature.Description;
            Tags = feature.Tags;
            Scenarios = feature.Scenarios;
            Background = feature.Background;
        }
    }
}
