// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Api.Managers;
using Augurk.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Augurk.Api.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{apiVersion:apiVersion}/dependencies")]
    public class DependencyController : Controller
    {
        private readonly IDependencyManager _dependencyManager;

        public DependencyController(IDependencyManager dependencyManager)
        {
            _dependencyManager = dependencyManager ?? throw new ArgumentNullException(nameof(dependencyManager));
        }

        [Route("")]
        [HttpGet]
        public Task<IEnumerable<FeatureGraph>> GetSystemWideDependencies()
        {
            return _dependencyManager.GetTopLevelFeatureGraphsAsync();
        }

        [Route("products/{productName}/features/{featureName}/versions/{version}")]
        [HttpGet]
        public Task<FeatureGraph> GetFeatureGraph(string productName, string featureName, string version)
        {
            return _dependencyManager.GetFeatureGraphAsync(productName, featureName, version);
        }
    }
}
