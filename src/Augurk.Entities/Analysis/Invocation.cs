/*
 Copyright 2018, Augurk
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

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
        public Invocation[] Invocations {get;set;}
    }
}
