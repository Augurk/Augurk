// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Augurk.Entities.Analysis
{
    /// <summary>
    /// Describes an invocation, as discover by one the Augurk automation analyzers.
    /// </summary>
    public class Invocation
    {
        /// <summary>
        /// Indicates which <see cref="InvocationKind">kind</see> of invocation is represented by this instance.
        /// </summary>
        public InvocationKind Kind { get; set; }

        /// <summary>
        /// The signature of the invoked method.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Whether the invocation is defined in source, but outside the specifications project.
        /// </summary>
        public bool Local { get; set; }

        /// <summary>
        /// The regular expressions which can be used to map steps to this invocation.
        /// </summary>
        /// <remarks>
        /// May be NULL or Empty if there are no mapped expressions.
        /// </remarks>
        public string[] RegularExpressions { get; set; }

        /// <summary>
        /// A list of signatures for interfaces implemented by this invocation.
        /// </summary>
        /// <remarks>
        /// May be NULL or empty if there are not interface definitions.
        /// </remarks>
        public string[] InterfaceDefinitions { get; set; }

        /// <summary>
        /// The actual target of the automation logic if annotated.
        /// </summary>
        public string[] AutomationTargets { get; set; }

        /// <summary>
        /// The ordered invocations made from with this invocation.
        /// </summary>
        public Invocation[] Invocations { get; set; }
    }
}
