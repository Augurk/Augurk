using NuGet.Versioning;
using System.Collections.Generic;

namespace Augurk.Api
{
    /// <summary>
    /// Compares strings by treating them as semantic versions and sorting them accordingly.
    /// </summary>
    internal class SemanticVersionComparer : IComparer<string>
    {
        /// <summary>
        /// Compares <paramref name="first"/> to <paramref name="second"/> and determines which one is bigger.
        /// </summary>
        /// <param name="first">First value to compare.</param>
        /// <param name="second">Second value to compare.</param>
        /// <returns>
        /// Returns -1 if <paramref name="first"/> is smaller than <paramref name="second"/>,
        /// 0 if <paramref name="first"/> and <paramref name="second"/> are equal
        /// and +1 if <paramref name="first"/> is bigger than <paramref name="second"/>.
        /// </returns>
        public int Compare(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) && !string.IsNullOrWhiteSpace(second))
            {
                // If the first string is null and the second is not, first is considered smaller than the second
                return -1;
            }
            else if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second))
            {
                // If both strings are null or whitespace, they are considered equal
                return 0;
            }
            else if (!string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(second))
            {
                // If the first string is not null and the second is, first is considered bigger than the second
                return 1;
            }

            // Try to parse both strings as semantic version
            SemanticVersion firstSemanticVersion;
            SemanticVersion secondSemanticVersion;
            bool firstSemanticVersionParseSucceeded = SemanticVersion.TryParse(first, out firstSemanticVersion);
            bool secondSemanticVersionParseSucceeded = SemanticVersion.TryParse(second, out secondSemanticVersion);

            if (!firstSemanticVersionParseSucceeded && secondSemanticVersionParseSucceeded)
            {
                // If the first string couldn't be parsed as a semantic version and the second could, the first is considered smaller than the second
                return -1;
            }
            else if (firstSemanticVersionParseSucceeded && !secondSemanticVersionParseSucceeded)
            {
                // If the first could be parsed as a semantic version and the second couldn't, the first is considered bigger than the second
                return 1;
            }
            else if (!firstSemanticVersionParseSucceeded && !secondSemanticVersionParseSucceeded)
            {
                // If both couldn't be parsed as a semantic version, compare them as strings
                return first.CompareTo(second);
            }
            else
            {
                // If both could be parsed as a semantic version, compare them as such
                return firstSemanticVersion.CompareTo(secondSemanticVersion);
            }
        }
    }
}
