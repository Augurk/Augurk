// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
        [Obsolete("Use Versions instead")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the versions of the feature.
        /// </summary>
        /// <remarks>
        /// All identical versions of a feature are regarded as a single feature.
        /// </remarks>
        public string[] Versions { get; set; }

        /// <summary>
        /// Gets or sets the hash of this feature.
        /// </summary>
        /// <remarks>
        /// The hash does not include the <see cref="Versions"/> or obsolete <see cref="Version"/> properties.
        /// </remarks>
        public string Hash { get; set; }

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
        public DbFeature(Feature feature, string product, string group, string parentTitle)
        {
            Product = product;
            Group = group;
            ParentTitle = parentTitle;

            // Copy the properties from the provided feature
            Title = feature.Title;
            Description = feature.Description;
            Tags = feature.Tags;
            Scenarios = feature.Scenarios;
            Background = feature.Background;
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
            Versions = new[] { version };
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
