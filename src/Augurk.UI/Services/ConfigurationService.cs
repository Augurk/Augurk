// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Augurk.Entities;
using Microsoft.AspNetCore.Components.Forms;

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

        public async Task ImportBackupAsync(IBrowserFile backupFile)
        {
            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(backupFile.OpenReadStream(int.MaxValue));
            content.Add(fileContent, "file", backupFile.Name);
            await _client.PostAsync("/api/v2/import", content);
        }
    }
}
