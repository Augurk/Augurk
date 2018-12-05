using Augurk.Api;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Embedded;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RavenDbServiceCollectionExtensions
    {
        public static void AddRavenDb(this IServiceCollection services)
        {
            var serverOptions = new ServerOptions
            {
                DataDirectory = Path.Combine(Environment.CurrentDirectory, "data")
            };

            var databaseOptions = new DatabaseOptions("AugurkStore")
            {
                Conventions = new DocumentConventions
                {
                    IdentityPartsSeparator = "-"
                }
            };

            EmbeddedServer.Instance.StartServer(serverOptions);

            IDocumentStore documentStore = EmbeddedServer.Instance.GetDocumentStore(databaseOptions);

            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), documentStore);

            services.AddSingleton<IDocumentStore>(documentStore);

            Database.DocumentStore = documentStore;
        }
    }
}
