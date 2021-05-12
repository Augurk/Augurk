/*
 Copyright 2020-2021, Augurk

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
using Augurk.Entities.Search;

public class SearchService {

    private readonly HttpClient _client;

    public SearchResults LatestResults {
        get; private set;
    }

    public bool Searching {
        get; private set;
    }

    public SearchService(HttpClient client)
    {
        _client = client;
    }

    public event Func<SearchResults, Task> OnSearchResultsChanged;
    public event Func<bool, Task> OnSearchStateChanged;

    public async Task Search(string searchInput)
    {
        if(string.IsNullOrWhiteSpace(searchInput))
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