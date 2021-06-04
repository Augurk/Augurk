// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Augurk.Entities.Search;

namespace Augurk.UI.Services
{
    public class SearchService
    {
        private readonly HttpClient _client;

        public SearchResults LatestResults { get; private set; }

        public bool Searching { get; private set; }

        public SearchService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public event Func<SearchResults, Task> OnSearchResultsChanged;
        public event Func<bool, Task> OnSearchStateChanged;

        public async Task Search(string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
            {
                LatestResults = null;
                Searching = false;
                await OnSearchStateChanged?.Invoke(Searching);
            }
            else
            {
                Searching = true;
                await OnSearchStateChanged?.Invoke(Searching);
                var results = await _client.GetFromJsonAsync<SearchResults>($"/api/v2/search?q={searchInput}");
                LatestResults = results;
                Searching = false;
                await OnSearchStateChanged?.Invoke(Searching);
            }

            await OnSearchResultsChanged?.Invoke(LatestResults);
        }
    }
}
