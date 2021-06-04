// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Entities;
using System;
using System.Text.RegularExpressions;
using Raven.Client.Documents.Session;
using Raven.Client;
using System.Globalization;

namespace Augurk
{
    internal static class AsyncDocumentSessionExtensions
    {
        public static void SetExpirationAccordingToConfiguration(this IAsyncDocumentSession session, object document, string version, Configuration configuration)
        {
            if (configuration.ExpirationEnabled &&
                    Regex.IsMatch(version, configuration.ExpirationRegex))
            {
                // Set the expiration in the metadata
                var metadata = session.Advanced.GetMetadataFor(document);
                var lastModifiedDate = DateTime.UtcNow;
                if (metadata.TryGetValue(Constants.Documents.Metadata.LastModified, out object value))
                {
                    lastModifiedDate = DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
                }
                metadata[Constants.Documents.Metadata.Expires] = lastModifiedDate.Date.AddDays(configuration.ExpirationDays);
            }
            else
            {
                // Remove the expiration if it is set
                var metadata = session.Advanced.GetMetadataFor(document);
                metadata.Remove(Constants.Documents.Metadata.Expires);
            }
        }

    }
}
