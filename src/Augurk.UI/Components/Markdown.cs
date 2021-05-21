using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Augurk.UI.Components
{
    public class Markdown : ComponentBase
    {
        [Parameter]
        public string Input { get; set; }
        private string rawHtml = string.Empty;

        public virtual MarkdownPipeline Pipeline => new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UseAdvancedExtensions()
            .Build();

        public Markdown()
        {
            var builder = new MarkdownPipelineBuilder()
            .UseEmojiAndSmiley()
            .UseAdvancedExtensions();
        }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            ComponentReplacer.ReplaceDiagrams(rawHtml)(builder);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrEmpty(Input))
            {
                rawHtml = Markdig.Markdown.ToHtml(Input, Pipeline);
            }
            else
            {
                rawHtml = string.Empty;
            }
        }
    }
}