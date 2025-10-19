namespace AStar.Dev.Logging.Extensions.Models;

/// <summary>
///     The <see cref="WriteTo" /> class that configures the relevant WriteTo Serilog logger.
/// </summary>
public sealed class WriteTo
{
    /// <summary>
    ///     The Name of the WriteTo option.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     An instance of <see cref="Args" /> to configure the WriteTo Serilog logger.
    /// </summary>
    public Args Args { get; set; } = new();
}
