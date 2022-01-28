// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
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

        private IEnumerable<Product> CachedProducts = null;

        public ProductService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var httpResult = await _client.GetAsync($"/api/v3/products/");
            if (!httpResult.IsSuccessStatusCode || httpResult.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            CachedProducts = await httpResult.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return CachedProducts;
        }

        public async Task<Product> GetProductAsync(string productName)
        {
            var products = CachedProducts ?? await GetProductsAsync();
            return products.SingleOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync(string productName)
        {
            var httpResult = await _client.GetAsync($"/api/v2/products/{productName}/groups/");
            if (!httpResult.IsSuccessStatusCode || httpResult.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            return await httpResult.Content.ReadFromJsonAsync<IEnumerable<Group>>();
        }
    }
}
