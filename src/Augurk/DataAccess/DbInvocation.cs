// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Api
{
    /// <summary>
    /// Describes an invication with al other invocations made from there
    /// </summary>
    public class DbInvocation
    {
        /// <summary>
        /// Gets or sets the signature of this invocation
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets a flat list of all signatures invoked
        /// by this invocation; both directly and indirectly.
        /// </summary>
        public string[] InvokedSignatures { get; set; }
    }
}
