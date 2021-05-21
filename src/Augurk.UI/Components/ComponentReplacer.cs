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

        public static RenderFragment ReplaceDiagrams(string rawContent)
        {
            return ApplyRegex(rawContent,
                              @"<pre><code class=""language-(?'type'\w+)"">(?:Label\:\W?(?'label'.*)\n)?(?'diagram'[\s\S]*?)<\/code><\/pre>",
                              ProcessDiagramMatch);
        }

        private static void ProcessDiagramMatch(Match match, RenderTreeBuilder builder)
        {
            switch(match.Groups["type"].Value)
            {
                case "UML":
                    builder.OpenComponent<UmlDisplay>(1);
                    builder.AddAttribute(1, nameof(UmlDisplay.Label), match.Groups["label"].Value);
                    builder.AddAttribute(1, nameof(UmlDisplay.DiagramText), match.Groups["diagram"].Value);
                    builder.CloseComponent();
                    break;
                case "sequence":
                    builder.OpenComponent<SequenceDisplay>(1);
                    builder.AddAttribute(1, nameof(SequenceDisplay.Label), match.Groups["label"].Value);
                    builder.AddAttribute(1, nameof(SequenceDisplay.DiagramText), match.Groups["diagram"].Value);
                    builder.CloseComponent();
                    break;
                default:
                    // The diagriam type is not supported, so just return the original content
                    builder.AddContent(0, match.Value);
                    break;
            }

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