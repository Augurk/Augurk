// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Augurk.UI.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Augurk.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBootstrapBlazor();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AugurkService>();
            builder.Services.AddScoped<ContextService>();
            builder.Services.AddScoped<SearchService>();
            builder.Services.AddScoped<FeatureService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<ConfigurationService>();

            await builder.Build().RunAsync();
        }
    }
}
