using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Augurk.UI.Components
{
    public class Markdown : ComponentBase
    {
        [Parameter]
        public string Input { get; set; }
        private MarkupString _markupString = new();

        public virtual MarkdownPipeline Pipeline => new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UseAdvancedExtensions()
            .Build();
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.AddContent(0, _markupString);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrEmpty(Input))
            {
                _markupString = new MarkupString(Markdig.Markdown.ToHtml(Input, Pipeline));
            }
            else
            {
                _markupString = new();
            }
        }
    }
}