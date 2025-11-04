namespace AStar.Dev.Logging.Extensions.Models;

/// <summary>
///     The <see cref="Override" /> class used to configure the Serilog logging level overrides.
/// </summary>
public sealed class Override
{
    /// <summary>
    ///     The Microsoft AspNetCore logging level.
    /// </summary>
    public string MicrosoftAspNetCore { get; set; } = string.Empty;

    /// <summary>
    ///     The <see cref="System.Net.Http" /> logging level.
    /// </summary>
    public string SystemNetHttp { get; set; } = string.Empty;

    /// <summary>
    ///     The AStar-specific logging level.
    /// </summary>
    public string AStar { get; set; } = string.Empty;
}
