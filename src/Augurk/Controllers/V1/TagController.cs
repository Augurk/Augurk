// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Augurk.Api.Managers;
using Augurk.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Augurk.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TagController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IFeatureManager _featureManager;

        public TagController(IProductManager productManager, IFeatureManager featureManager)
        {
            _productManager = productManager ?? throw new ArgumentNullException(nameof(productManager));
            _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        }

        [Route("api/tags/{branchName}")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync(string branchName)
        {
            return FeatureProcessor.RemoveServerTags(await _productManager.GetTagsAsync(branchName));
        }

        [Route("api/tags/{branchName}/{tag}/features")]
        [HttpGet]
        public async Task<IEnumerable<FeatureDescription>> GetFeaturesAsync(string branchName, string tag)
        {
            return await _featureManager.GetFeatureDescriptionsByBranchAndTagAsync(branchName, tag);
        }
    }
}
