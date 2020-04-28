using System;
using System.Threading.Tasks;
using Augurk.Entities.Search;

public class SearchService {

    private string _searchInput;

    public SearchResults LatestResults {
        get; private set;
    }

    public event Func<SearchResults, Task> OnSearchResultsChanged;

    public async Task Search(string searchInput)
    {
        _searchInput = searchInput;

        await Task.Delay(500);

        // Don't do anything if the input has changed
        if(_searchInput != searchInput)
        {
            return;
        }

        var results = new SearchResults() {
            SearchQuery = searchInput,
            FeatureMatches = new [] { new FeatureMatch{ FeatureName = "Feature 1" }, new FeatureMatch{ FeatureName = "Feature 2" }}
        };

        LatestResults = results;

        if(OnSearchResultsChanged != null) {
            await OnSearchResultsChanged.Invoke(results);
        }
    }
}