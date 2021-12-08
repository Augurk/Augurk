// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Augurk.Entities;
using Augurk.Entities.Search;

namespace Augurk.UI.Services
{
    public class ContextService
    {
        public Product CurrentProduct { get; private set; }

        public event Func<Product, Task> OnProductChanged;

        public async Task SetCurrentProduct(Product product)
        {
            CurrentProduct = product;
            await OnProductChanged?.Invoke(CurrentProduct);
        }
    }
}
