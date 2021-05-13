/*
 Copyright 2014, Mark Taling
 
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

using System;

namespace Augurk.Entities
{
    /// <summary>
    /// Represents the different properties of a feature
    /// </summary>
    [Flags]
    public enum FeatureProperties
    {
        /// <summary>
        /// This feature has no properties
        /// </summary>
        None = 0,

        /// <summary>
        /// This feature should be ignored by the testengine
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// This feature has not been implemented
        /// </summary>
        NotImplemented = 2,
    }
}