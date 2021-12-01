// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.UI.Services
{
    public class ProductService
    {
        private readonly HttpClient _client;

        public ProductService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var httpResult = await _client.GetAsync($"/api/v3/products/");
            Console.WriteLine(httpResult.IsSuccessStatusCode);
           
            if(!httpResult.IsSuccessStatusCode || httpResult.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            var products = await httpResult.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return products;
        }
    }
}
