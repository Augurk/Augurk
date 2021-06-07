// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Raven.Client.Documents;

namespace Augurk
{
    public interface IDocumentStoreProvider
    {
        IDocumentStore Store { get; }
    }
}
