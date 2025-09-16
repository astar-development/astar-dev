using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public class MarkdownViewBase : ComponentBase
{
    [Parameter]
    public string? Content { get; set; }

    // When true, raw HTML inside Markdown is NOT rendered (sanitized in JS).
    [Parameter]
    public bool DisableHtml { get; set; } = true;

    protected string _html = string.Empty;

    [Inject]
    protected IJSRuntime JS { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        var md = Content ?? string.Empty;

        // Render markdown via JS (marked + DOMPurify). If DisableHtml = true, we sanitize.
        var rendered = await JS.InvokeAsync<string>(
                                                    "renderMarkdown",
                                                    md,
                                                    !DisableHtml // allowHtml
                                                   );

        _html = rendered;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) =>

        // Re-highlight after each render so newly rendered code gets styled
        await JS.InvokeVoidAsync("highlightMarkdown");
}
