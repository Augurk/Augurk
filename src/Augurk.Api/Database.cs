using Raven.Client;

namespace Augurk.Api
{
    /// <summary>
    /// A static accesspoint for the database
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// Gets or sets the documentstore which should be used to access the features.
        /// </summary>
        public static IDocumentStore DocumentStore { get; set; } 
    }
}
