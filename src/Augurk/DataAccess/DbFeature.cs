/*
 Copyright 2015-2019, Augurk
 
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
using System.Collections.Generic;

namespace Augurk.Api
{
    /// <summary>
    /// An implementation of <see cref="Feature"/> designed specifically for storage in the database.
    /// </summary>
    public class DbFeature : Feature
    {
        /// <summary>
        /// Gets or sets the product to which this feature belongs.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the title of the group this feature falls under.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the version of the feature.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the title of the parent feature.
        /// </summary>
        public string ParentTitle { get; set; }

        /// <summary>
        /// Gets or sets the the test result.
        /// </summary>
        public FeatureTestResult TestResult { get; set; }

        /// <summary>
        /// Gets or sets the signatures of the direct invocations performed by this feature.
        /// </summary>
        public List<string> DirectInvocationSignatures { get; set; }

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
        /// <param name="group">The group this feature falls under.</param>
        /// <param name="parentTitle">The title of the parent feature.</param>
        /// <param name="version">Version of the feature.</param>
        /// <remarks>This constructor does not wrap the provided feature, it will result in an actual copy with shared scenarios and a shared background.</remarks>
        public DbFeature(Feature feature, string product, string group, string parentTitle, string version)
        {
            Product = product;
            Group = group;
            Version = version;
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
