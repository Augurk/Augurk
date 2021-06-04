// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

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
            foreach (var value in source)
            {
                // Yield the current item
                yield return value;

                var enumerable = childSelector(value);

                // Descent children of the item using the given function
                if (enumerable != null)
                {
                    foreach (var child in enumerable.Descendants(childSelector))
                    {
                        // Yield the child
                        yield return child;
                    }
                }
            }
        }
    }
}
