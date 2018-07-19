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
    /// Represents the different invocation kinds supported by Augurk
    /// </summary>
    public enum InvocationKind
    {
        /// <summary>
        /// Default value
        /// </summary>
        None = 0,

        /// <summary>
        /// The invocation represents an automated When step
        /// </summary>
        When = 1,

        /// <summary>
        /// The method invoked is marked as public
        /// </summary>
        Public = 2,

        /// <summary>
        /// The method invoked is marked as private
        /// </summary>
        Private = 4,

        /// <summary>
        /// The method invoked is marked as internal 
        /// </summary>
        Internal = 8
    }
}
