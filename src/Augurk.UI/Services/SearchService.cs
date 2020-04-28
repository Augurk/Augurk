using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Augurk.Entities.Search;

public class SearchService {

    private HttpClient _client;

    public SearchResults LatestResults {
        get; private set;
    }

    public SearchService(HttpClient client)
    {
        _client = client;
    }

    public event Func<SearchResults, Task> OnSearchResultsChanged;

    public async Task Search(string searchInput)
    {
        if(String.IsNullOrWhiteSpace(searchInput)) {
            LatestResults = null;
        }
        else {
            var results = await _client.GetFromJsonAsync<SearchResults>($"/api/v2/search?q={searchInput}");

            LatestResults = results;
        }

        if(OnSearchResultsChanged != null) {
            await OnSearchResultsChanged.Invoke(LatestResults);
        }
    }
}