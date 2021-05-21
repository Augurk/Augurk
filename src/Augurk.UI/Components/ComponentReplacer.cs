using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Augurk.UI.Components
{
    public static class ComponentReplacer
    {
        public static RenderFragment ReplaceArguments(string rawContent)
        {
            return ApplyRegex(rawContent,
                              @"<(\w+)>",
                              (match, builder) => {
                                  builder.OpenComponent<ArgumentDisplay>(1);
                                  builder.AddAttribute(1, nameof(ArgumentDisplay.ArgumentName), match.Groups[1].Value);
                                  builder.CloseComponent();
                              });
        }

        public static RenderFragment ReplaceUml(string rawContent)
        {
            return ApplyRegex(rawContent,
                              @"<pre><code class=""language-UML"">(?:Label\:\W?(?'label'.*)\n)?(?'diagram'[\s\S]*?)<\/code><\/pre>",
                              (match, builder) => {
                                  builder.OpenComponent<UmlDisplay>(1);
                                  builder.AddAttribute(1, nameof(UmlDisplay.Label), match.Groups["label"].Value);
                                  builder.AddAttribute(1, nameof(UmlDisplay.DiagramText), match.Groups["diagram"].Value);
                                  builder.CloseComponent();
                              });
        }

        private static RenderFragment ApplyRegex(string rawContent, string regex, Action<Match, RenderTreeBuilder> replacer)
        {
            var matches = Regex.Matches(rawContent, regex);
            if (!matches.Any())
            {
                return builder => builder.AddContent(0, new MarkupString(rawContent));
            }

            return builder =>
            {
                int processedUntilIndex = 0;

                foreach (Match match in matches)
                {
                    builder.AddContent(0, new MarkupString(rawContent.Substring(processedUntilIndex, match.Index - processedUntilIndex)));
                    processedUntilIndex = match.Index + match.Length;

                    replacer(match, builder);
                }

                builder.AddContent(0, new MarkupString(rawContent.Substring(processedUntilIndex, rawContent.Length - processedUntilIndex)));
            };
        }
    }
}