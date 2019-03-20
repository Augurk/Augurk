/*
 Copyright 2019, Augurk

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
using System.Reflection;

namespace Augurk.Api
{
    /// <summary>
    /// Contains utility methods for anything related to the environment Augurk is running in.
    /// </summary>
    public static class EnvironmentUtils
    {
        /// <summary>
        /// Gets the version of .NET Core that Augurk is currently running on.
        /// </summary>
        /// <returns>Returns a string containing the major.minor.patch of the .NET Core version Augurk is currently running on.</returns>
        public static string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                return assemblyPath[netCoreAppIndex + 1];
            return null;
        }
    }
}