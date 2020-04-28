using System;
using System.Threading.Tasks;
using Augurk.Entities.Search;

public class SearchService {
    public SearchResults LatestResults {
        get; private set;
    }

    public event Func<SearchResults, Task> OnSearchResultsChanged;

    public async Task Search(string searchInput)
    {
        var results = new SearchResults() {
            SearchQuery = searchInput,
            FeatureMatches = new [] {
                new FeatureMatch{ FeatureName = "Feature 1", MatchingText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer mollis dui sed eros consectetur aliquet. Donec varius libero ligula, ut fermentum tellus porttitor non." },
                new FeatureMatch{ FeatureName = "Feature with a very long name which mostly will not fit the screen", MatchingText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer mollis dui sed eros consectetur aliquet. Donec varius libero ligula, ut fermentum tellus porttitor non." }}
        };

        LatestResults = results;

        if(OnSearchResultsChanged != null) {
            await OnSearchResultsChanged.Invoke(results);
        }
    }
}