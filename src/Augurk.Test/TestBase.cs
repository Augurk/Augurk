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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Documents;
using Raven.TestDriver;

namespace Augurk.Test
{
    /// <summary>
    /// Abstract base class used for testing.
    /// </summary>
    public abstract class TestBase : RavenTestDriver
    {
        /// <summary>
        /// Called before the document store is being initialized.
        /// </summary>
        /// <param name="documentStore">A <see cref="IDocumentStore" /> instance to configure.</param>
        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.Conventions.IdentityPartsSeparator = "-";
        }

        /// <summary>
        /// Called before each test execution to setup dependency injection.
        /// </summary>
        [TestInitialize]
        public virtual void TestInitialize()
        {
            var documentStore = GetDocumentStore();
            var services = new ServiceCollection();
            services.AddSingleton<IDocumentStoreProvider>(new TestDocumentStoreProvider(documentStore));
            services.AddManagers();

            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider" /> used to request service implementation.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; private set; }
    }
}