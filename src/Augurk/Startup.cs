using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Augurk
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add core MVC services and setup the versioned API explorer
            services.AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddVersionedApiExplorer(options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    });
            
            // Add the rest of the MVC stack (which we apparently need for now)
            services.AddMvc();

            // Setup API versioning
            services.AddApiVersioning(options => options.AssumeDefaultVersionWhenUnspecified = true);

            // Add generation of Swagger documents
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }
            });

            // Setup RavenDB (Embedded)
            services.AddRavenDb();

            // Add our own managers
            services.AddManagers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseFileServer();

            // Add Swagger support at the appropriate endpoints
            app.UseSwagger(options => options.RouteTemplate = "doc/api/{documentName}");
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "doc/ui";

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/doc/api/{description.GroupName}", description.GroupName.ToUpperInvariant());
                }
            });
        }

        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Version = description.ApiVersion.ToString(),
                Title = description.GroupName.ToUpperInvariant() == "V1" ? 
                    "Augurk Branch based API" : 
                    "Augurk Product based API",
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
