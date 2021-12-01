// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Augurk.UI.Services
{
    public class AugurkService
    {
        private readonly HttpClient _client;
        private string _version;

        public AugurkService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<string> GetVersionAsync()
        {
            if (!string.IsNullOrWhiteSpace(_version))
            {
                return _version;
            }

            _version = await _client.GetStringAsync("/api/version");
            return _version;
        }
    }
}
