/*
 Copyright 2018-2019, Augurk

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
using System.Threading.Tasks;
using Alba;
using Augurk.Api;
using Augurk.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;

namespace Augurk.IntegrationTest
{
    /// <summary>
    /// Abstract base class used for testing.
    /// </summary>
    public abstract class TestBase : RavenTestDriver
    {
        private readonly SystemUnderTestFixture _fixture;

        /// <summary>
        /// Pre-configures the RavenDb in memory test server.
        /// </summary>
        static TestBase()
        {
            string dotNetCoreVersion = EnvironmentUtils.GetNetCoreVersion();

            RavenTestDriver.ConfigureServer(new TestServerOptions
            {
                FrameworkVersion = dotNetCoreVersion
            });

            Console.WriteLine($"Configured RavenDb in memory test driver to use version {dotNetCoreVersion} of .NET Core.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase" /> class.
        /// </summary>
        /// <param name="fixture">A <see cref="SystemUnderTestFixture" /> instance to use.</param>
        protected TestBase(SystemUnderTestFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _fixture.Store = GetDocumentStore();
            IndexCreation.CreateIndexes(typeof(Startup).Assembly, _fixture.Store);
        }

        /// <summary>
        /// Called before the document store is being initialized.
        /// </summary>
        /// <param name="documentStore">A <see cref="IDocumentStore" /> instance to configure.</param>
        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.Conventions.IdentityPartsSeparator = "-";
        }

        /// <summary>
        /// Gets the <see cref="SystemUnderTest" />.
        /// </summary>
        protected SystemUnderTest System { get { return _fixture.System; } }

        /// <summary>
        /// Gets the <see cref="IDocumentStore" />.
        /// </summary>
        protected IDocumentStore Store { get { return _fixture.Store; } }
    }
}