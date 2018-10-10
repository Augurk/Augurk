/*
 Copyright 2018, Augurk
 
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

namespace Augurk.Entities
{
    public class FeatureGraph
    {
        public string FeatureName { get; set; }

        public string ProductName { get; set; }

        public string GroupName { get; set; }

        public string Version { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public List<FeatureGraph> DependsOn { get; set; } = new List<FeatureGraph>();

        public List<FeatureGraph> Dependants { get; set; } = new List<FeatureGraph>();

        public bool DescribesSameFeature(FeatureGraph other)
        {
            return this.FeatureName.Equals(other.FeatureName, StringComparison.Ordinal) &&
                   this.ProductName.Equals(other.ProductName, StringComparison.Ordinal) &&
                   this.Version.Equals(other.Version, StringComparison.Ordinal);
        }
    }
}
