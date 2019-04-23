using Alba;
using Augurk.Entities;

namespace Augurk.IntegrationTest
{
    public static class TestHelper
    {
        public static SendExpression PostFeature(this Alba.Scenario scenario, Feature feature, string productName, string groupName)
        {
            return scenario.Post.Json(feature).ToUrl(GenerateFeatureUrl(productName, groupName, feature.Title));
        }

        public static SendExpression GetFeature(this Alba.Scenario scenario, string productName, string groupName, string title)
        {
            return scenario.Get.Url(GenerateFeatureUrl(productName, groupName, title));
        }

        public static SendExpression PutProductDescription(this Alba.Scenario scenario, string productName, string productDescription)
        {
            return scenario.Put.Text(productDescription).ToUrl(GenerateProductUrl(productName) + "/description");
        }

        public static SendExpression GetProductDescription(this Alba.Scenario scenario, string productName)
        {
            return scenario.Get.Url(GenerateProductUrl(productName) + "/description");
        }

        public static string GenerateProductUrl(string productName)
        {
            return $"/api/v2/products/{productName}";
        }

        public static string GenerateFeatureUrl(string productName, string groupName, string title, string version = "0.0.0")
        {
            return $"/api/v2/products/{productName}/groups/{groupName}/features/{title}/versions/{version}";
        }
    }
}