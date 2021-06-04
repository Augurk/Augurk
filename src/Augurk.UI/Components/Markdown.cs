// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Augurk.UI.Components
{
    public class Markdown : ComponentBase
    {
        [Parameter]
        public string Input { get; set; }
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
                _rawHtml = Markdig.Markdown.ToHtml(Input, Pipeline);
            }
            else
            {
                _rawHtml = string.Empty;
            }
        }
    }
}
