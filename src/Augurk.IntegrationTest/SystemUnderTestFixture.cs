using System;
using Alba;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;

namespace Augurk.IntegrationTest
{
    /// <summary>
    /// Provides access to the <see cref="SystemUnderTest" /> by sharing it accross multiple tests
    /// avoiding the need to initialize the ASP.NET Core stack for every test.
    /// </summary>
    public sealed class SystemUnderTestFixture : IDisposable, IDocumentStoreProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemUnderTest" />.
        /// </summary>
        public SystemUnderTestFixture()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .UseEnvironment("Production")
                .ConfigureLogging(logging =>
                {
                    // Disable logging for built-in stuff
                    logging.AddFilter("System", LogLevel.Error);
                    logging.AddFilter("Microsoft", LogLevel.Error);
                })
                .ConfigureServices(services =>
                {
                    // Use this instance as the IDocumentStoreProvider
                    services.AddSingleton<IDocumentStoreProvider>(this);
                })
                .ConfigureWebHost(configure =>
                {
                    configure.UseStartup<Startup>();
                });

            System = new AlbaHost(builder);
        }

        /// <summary>
        /// Disposes any claimed resources.
        /// </summary>
        public void Dispose()
        {
            System?.Dispose();
            Store?.Dispose();
        }

        /// <summary>
        /// Gets the <see cref="AlbaHost" />.
        /// </summary>
        public AlbaHost System { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="IDocumentStore" /> to use.
        /// </summary>
        public IDocumentStore Store { get; set; }
    }
}
