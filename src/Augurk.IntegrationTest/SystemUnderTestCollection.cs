using Xunit;

namespace Augurk.IntegrationTest
{
    [CollectionDefinition(nameof(SystemUnderTestCollection))]
    public class SystemUnderTestCollection : ICollectionFixture<SystemUnderTestFixture>
    {
    }
}