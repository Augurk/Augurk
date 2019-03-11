namespace Augurk.IntegrationTest
{
    public static class TestHelper
    {
        public static string GenerateFeatureUrl(string productName, string groupName, string title, string version = "0.0.0")
        {
            return $"/api/v2/products/{productName}/groups/{groupName}/features/{title}/versions/{version}";
        }
    }
}