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
