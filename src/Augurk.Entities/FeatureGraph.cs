// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
            return FeatureName.Equals(other.FeatureName, StringComparison.Ordinal) &&
                   ProductName.Equals(other.ProductName, StringComparison.Ordinal) &&
                   Version.Equals(other.Version, StringComparison.Ordinal);
        }
    }
}
