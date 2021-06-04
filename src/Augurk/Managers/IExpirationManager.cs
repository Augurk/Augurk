// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Augurk.Entities;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Defines methods for expiring documents.
    /// </summary>
    public interface IExpirationManager
    {
        Task ApplyExpirationPolicyAsync(Configuration configuration);
    }
}
