/*
 Copyright 2019-2020, Augurk

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
using System.Reflection;
using System.Runtime.Versioning;
using Augurk.Api;
using Augurk.Api.Indeces;
using NSubstitute;
using Raven.Client.Documents;
using Raven.TestDriver;

namespace Augurk.Test
{
    /// <summary>
    /// Abstract base class for tests that interact with RavenDb.
    /// </summary>
    public abstract class RavenTestBase : RavenTestDriver
    {
        private readonly Lazy<IDocumentStoreProvider> _documentStoreProvider;

        /// <summary>
        /// Returns the document store provider for the current test.
        /// </summary>
        /// <value></value>
        protected IDocumentStoreProvider DocumentStoreProvider
        {
            get { return _documentStoreProvider.Value; }
        }

        /// <summary>
        /// Returns the document store for the current test.
        /// </summary>
        /// <value></value>
        protected IDocumentStore DocumentStore
        {
            get { return DocumentStoreProvider.Store; }
        }

        /// <summary>
        /// Pre-configures the RavenDb in memory test server.
        /// </summary>
        static RavenTestBase()
        {
            var dotNetVersion = Environment.Version.ToString();

            RavenTestDriver.ConfigureServer(new TestServerOptions
            {
                FrameworkVersion = dotNetVersion
            });

            System.Console.WriteLine($"Configured RavenDb in memory test driver to use version {dotNetVersion} of .NET Core.");
        }

        public RavenTestBase()
        {
            _documentStoreProvider = new Lazy<IDocumentStoreProvider>(GetDocumentStoreProvider);
        }

        /// <summary>
        /// Called before the document store is being initialized.
        /// </summary>
        /// <param name="documentStore">A <see cref="IDocumentStore" /> instance to configure.</param>
        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.Conventions.IdentityPartsSeparator = '-';
        }

        /// <summary>
        /// Gets a <see cref="IDocumentStoreProvider" /> implementation pre-configured to return the <see cref="IDocumentStore" />
        /// that's in scope for the current test.
        /// </summary>
        private IDocumentStoreProvider GetDocumentStoreProvider()
        {
            var documentStore = GetDocumentStore();

            // Prepare all indeces
            documentStore.ExecuteIndex(new Features_ByProductAndBranch());
            documentStore.ExecuteIndex(new Features_ByTitleProductAndGroup());
            documentStore.ExecuteIndex(new Features_WithoutHash());
            documentStore.ExecuteIndex(new Products_ByName());

            // Wait for all indeces to be done
            WaitForIndexing(documentStore);

            var documentStoreProvider = Substitute.For<IDocumentStoreProvider>();
            documentStoreProvider.Store.Returns(documentStore);

            return documentStoreProvider;
        }
    }
}