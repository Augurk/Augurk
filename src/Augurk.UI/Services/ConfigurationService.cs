// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.UI.Services
{
    public class ConfigurationService
    {
        private readonly HttpClient _client;

        public ConfigurationService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Configuration> GetConfigurationAsync()
        {
            return await _client.GetFromJsonAsync<Configuration>("/api/v2/configuration");
        }

        public async Task SaveConfigurationAsync(Configuration configuration)
        {
            await _client.PostAsJsonAsync("/api/v2/configuration", configuration);
        }

        public async Task<Customization> GetCustomizationAsync()
        {
            return await _client.GetFromJsonAsync<Customization>("/api/v2/customization");
        }

        public async Task SaveCustomizationAsync(Customization customization)
        {
            await _client.PostAsJsonAsync("/api/v2/customization", customization);
        }
    }
}
