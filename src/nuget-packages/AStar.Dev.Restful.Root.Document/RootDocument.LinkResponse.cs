using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Restful.Root.Document;

/// <summary>
///     The <see cref="LinkResponse" /> class containing the HATEOAS links.
/// </summary>
[Refactor(1, 1, "Do we still use this? If so / if not - should we?")]
public sealed class LinkResponse
{
    /// <summary>
    ///     The Rel link
    /// </summary>
    public string Rel { get; set; } = string.Empty;

    /// <summary>
    ///     The HRef <see langword="for" />the link
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    ///     The applicable HTTP Method (GET, POST, etc.)
    /// </summary>
    public string Method { get; set; } = "GET";
}
