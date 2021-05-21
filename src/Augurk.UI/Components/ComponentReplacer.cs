using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Augurk.UI.Components
{
    public static class ComponentReplacer
    {
        public static RenderFragment ReplaceArguments(string rawContent)
        {
            var matches = Regex.Matches(rawContent, @"<(\w+)>");
            if (!matches.Any())
            {
                return builder => builder.AddContent(1, rawContent);
            }

            return builder =>
            {
                Match previousMatch = null;
                foreach (var match in matches.Cast<Match>())
                {
                    builder.AddContent(1, rawContent.Substring(previousMatch?.Index ?? 0, match.Index));
                    builder.OpenComponent<ArgumentDisplay>(1);
                    builder.AddAttribute(1, nameof(ArgumentDisplay.ArgumentName), match.Groups[1].Value);
                    builder.CloseComponent();

                    previousMatch = match;
                }

                builder.AddContent(1, rawContent.Substring((previousMatch?.Index ?? 0) + previousMatch.Length));
            };
        }
    }
}