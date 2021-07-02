// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Augurk.UI.Components
{
    public delegate string HandleLink(string linkText);

    public class Markdown : ComponentBase
    {
        [Parameter]
        public string Input { get; set; }

        [Parameter]
        public HandleLink LinkHandler { get; set; }

        private string _rawHtml = string.Empty;

        public virtual MarkdownPipeline Pipeline => new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UseAdvancedExtensions()
            .Build();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            ComponentReplacer.ReplaceDiagrams(_rawHtml)(builder);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrEmpty(Input))
            {
                _rawHtml = Markdig.Markdown.ToHtml(ParseFeatureLinks(Input), Pipeline);
            }
            else
            {
                _rawHtml = string.Empty;
            }
        }

        private string ParseFeatureLinks(string rawContent)
        {
            const string regex = @"\[(?<feature>[^\]]+)\]\z|\[(?<feature>[^\]]+)\](?=[^\(])";
            StringBuilder sb = new StringBuilder();
            int prevPosition = 0;
            foreach(Match match in Regex.Matches(rawContent, regex))
            {
                sb.Append(rawContent.Substring(prevPosition, match.Index - prevPosition));
                sb.Append(match.Value);
                // Only encode the whitespaces, as the markdown handler does not appreciate them
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0})", LinkHandler(match.Groups["feature"].Value).Replace(" ", "%20"));

                prevPosition = match.Index + match.Length;
            }
            sb.Append(rawContent.Substring(prevPosition));

            return sb.ToString();
        }
    }
}
