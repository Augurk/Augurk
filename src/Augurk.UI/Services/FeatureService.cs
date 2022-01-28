// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;

using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.UI.Services
{
    public class FeatureService
    {
        private readonly HttpClient _client;

        public FeatureService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<DisplayableFeature> GetFeatureAsync(string product, string group, string featureTitle, string version)
        {
            var httpResult = await _client.GetAsync($"/api/v2/products/{product}/groups/{group}/features/{featureTitle}/versions/{version}");
            if (!httpResult.IsSuccessStatusCode || httpResult.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            var feature = await httpResult.Content.ReadFromJsonAsync<DisplayableFeature>();
            return feature;
        }
    }
}
