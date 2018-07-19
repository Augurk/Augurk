/*
 Copyright 2015, 2017, Augurk
 
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;
using Augurk.Api.Filters;
using Augurk.Api.Formatters;
using Augurk.Api.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Swagger.Net.Application;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Augurk.Api
{
    /// <summary>
    /// OWIN startup class that configures the application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Called when the supplied <paramref name="app"/> is to be configured.
        /// </summary>
        /// <param name="app">An <see cref="IAppBuilder"/> instance that represents the application.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Configuration is used by infrastructure, so must be implemented this way.")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Called by infrastructure, so must be implemented this way.")]
        public void Configuration(IAppBuilder app)
        {
            // Before configuring the API, first initialize the database
            InitializeRavenDB();

            // Build the configuration for Web API
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // Ensure no browser-side caching is used
            config.Filters.Add(new NoCacheHeaderFilter());

            // Make JSON the default format
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            // Use camel-casing for property names
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // Serialize enums using their string value
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            // Be flexible with additional data, for backwards compatability purposes
            config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            // Allow for plaintext
            config.Formatters.Insert(0, new TextMediaTypeFormatter());

            // Share the formatter settings with the manager
            FeatureManager.JsonSerializerSettings = config.Formatters.JsonFormatter.SerializerSettings;

            // Make sure that Web API is enabled for our application
            app.UseWebApi(config);

            // Set the error detail policy to match the asp.net configuration
            SetErrorDetailPolicy(config);

            // Enable documentation
            config.EnableSwagger("doc/api/{apiVersion}", c => c.MultipleApiVersions(
                (description, version) =>
                {
                    if (version == "V2")
                    {
                        return description.RelativePath.StartsWith("api/v2", StringComparison.InvariantCultureIgnoreCase);
                    }

                    // Because V1 does not contain the version we can only check wether it isn't a V2
                    return !description.RelativePath.StartsWith("api/v2", StringComparison.InvariantCultureIgnoreCase);
                },
                (versionbuilder) =>
                {
                    versionbuilder.Version("V1", "Augurk Branch based API (Legacy)");
                    versionbuilder.Version("V2", "Augurk Product based API");
                })).EnableSwaggerUi("doc/ui/{*assetPath}", c => c.EnableDiscoveryUrlSelector());
        }

        /// <summary>
        /// Sets the Error Detail Policy based on the customErrors setting in the config.
        /// </summary>
        /// <param name="config">The configuration.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "customErrors", Justification = "Valid reference to XML Element.")]
        private void SetErrorDetailPolicy(HttpConfiguration config)
        {
            var configSection = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");

            IncludeErrorDetailPolicy errorDetailPolicy;

            switch (configSection.Mode)
            {
                case CustomErrorsMode.RemoteOnly:
                    errorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
                    break;
                case CustomErrorsMode.On:
                    errorDetailPolicy = IncludeErrorDetailPolicy.Never;
                    break;
                case CustomErrorsMode.Off:
                    errorDetailPolicy = IncludeErrorDetailPolicy.Always;
                    break;
                default:
                    throw new NotSupportedException(String.Format(CultureInfo.InvariantCulture,
                                                                  "The customErrors mode '{0}' is not supported.",
                                                                  configSection.Mode));
            }

            config.IncludeErrorDetailPolicy = errorDetailPolicy;
        }


        /// <summary>
        /// Initialized RavenDB based upon the configuration.
        /// </summary>
        private void InitializeRavenDB()
        {
            Database.DocumentStore = new EmbeddableDocumentStore
            {
                ConnectionStringName = "RavenDB",
                Configuration =
                {
                    Settings =
                    {
                        {"Raven/ActiveBundles", "DocumentExpiration"}
                    }
                }
            };

            Database.DocumentStore.Conventions.IdentityPartsSeparator = "-";
            Database.DocumentStore.Initialize();
            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Database.DocumentStore);
        }
    }
}