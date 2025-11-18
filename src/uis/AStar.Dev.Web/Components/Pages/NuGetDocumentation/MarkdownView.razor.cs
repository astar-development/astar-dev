using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AStar.Dev.Web.Components.Pages.NuGetDocumentation;

public partial class MarkdownView : ComponentBase
{
    protected string Html = string.Empty;

    [Parameter] public string? Content { get; set; }

    // When true, raw HTML inside Markdown is NOT rendered (sanitized in JS).
    [Parameter] public bool DisableHtml { get; set; } = true;

    [Inject] protected IJSRuntime Js { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        var md = Content ?? string.Empty;

        // Render markdown via JS (marked + DOMPurify). If DisableHtml = true, we sanitize.
        var rendered = await Js.InvokeAsync<string>(
            "renderMarkdown",
            md,
            !DisableHtml // allowHtml
        );

        Html = rendered;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)

        // Re-highlight after each render so newly rendered code gets styled
        => await Js.InvokeVoidAsync("highlightMarkdown");
}
