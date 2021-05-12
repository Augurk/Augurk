/*
 Copyright 2021, Augurk

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

        public async Task<Feature> GetFeatureAsync(string product, string group, string featureTitle, string version)
        {
            var feature = await _client.GetFromJsonAsync<Feature>($"/api/v2/products/{product}/groups/{group}/features/{featureTitle}/versions/{version}");
            return feature;
        }
    }
}