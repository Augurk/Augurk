/*
 Copyright 2017, Augurk
 
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
using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqExtensions
    {
        /// <summary>
        /// Recursively go through a list of items in <paramref name="source"/> and collect child items using <paramref name="childSelector"/>.
        /// </summary>
        /// <typeparam name="T">Type of items in <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> of items to iterate over.</param>
        /// <param name="childSelector">A <see cref="Func{T, TResult}"/> that selects child elements for each item in <paramref name="source"/>.</param>
        /// <returns>Returns a range of items from <paramref name="source"/> including child items selected by <paramref name="childSelector"/>.</returns>
        public static IEnumerable<T> Descendants<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            // Validate arguments
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (childSelector == null)
            {
                throw new ArgumentNullException(nameof(childSelector));
            }

            return DescendantsIterator(source, childSelector);
        }

        private static IEnumerable<T> DescendantsIterator<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            // Go through each item in the source
            foreach (T value in source)
            {
                // Yield the current item
                yield return value;

                IEnumerable<T> enumerable = childSelector(value);

                // Descent children of the item using the given function
                if (enumerable != null)
                {
                    foreach (T child in enumerable.Descendants(childSelector))
                    {
                        // Yield the child
                        yield return child;
                    }
                }
            }
        }
    }
}
