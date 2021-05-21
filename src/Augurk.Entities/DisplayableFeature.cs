/*
 Copyright 2014-2015, Mark Taling

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

using System.Linq;
using Augurk.Entities.Test;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents a feature, extended with display specific details.
    /// </summary>
    public class DisplayableFeature : Feature
    {
        /// <summary>
        /// Gets or sets the version of this feature.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the properties for this feature.
        /// </summary>
        public FeatureProperties Properties { get; set; }

        /// <summary>
        /// Gets or sets the the test result
        /// </summary>
        public FeatureTestResult TestResult { get; set; }

        /// <summary>
        /// Parameterless constructor for deserialization purposes.
        /// </summary>
        public DisplayableFeature()
        {
            // Nothing to do here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayableFeature"/> class using the provided <see cref="Feature"/> to provide initial values.
        /// </summary>
        /// <param name="feature">The <see cref="Feature"/>that should be used when determining the the initial values.</param>
        /// <remarks>This constructor does not wrap the provided feature, it will result in an actual copy with shared scenarios and a shared background.</remarks>
        public DisplayableFeature(Feature feature)
        {
            // Copy the properties from the provided feature
            Title = feature.Title;
            Description = feature.Description;
            Tags = feature.Tags.ToList();
            Scenarios = feature.Scenarios;
            Background = feature.Background;
        }
    }
}