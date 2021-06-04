// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Security.Cryptography;
using System.Text;

namespace Augurk.Api
{
    public static class DbProductExtensions
    {
        public static string GetIdentifier(this DbProduct product)
        {
            return GetIdentifier(product.Name);
        }

        public static string GetIdentifier(string productName)
        {
            using var cryptoServiceProvider = new MD5CryptoServiceProvider();
            var bytes = Encoding.Default.GetBytes(productName);
            var guid = new Guid(cryptoServiceProvider.ComputeHash(bytes));
            return guid.ToString("D");
        }
    }
}
