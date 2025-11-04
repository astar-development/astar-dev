namespace AStar.Dev.Logging.Extensions.Models;

/// <summary>
///     The <see cref="SerilogConfig" /> class that contains the Serilog configuration.
/// </summary>
public sealed class SerilogConfig
{
    /// <summary>
    ///     The <see cref="Serilog" /> class containing the core configuration.
    /// </summary>
    public Serilog Serilog { get; set; } = new();

    /// <summary>
    ///     The <see cref="Logging" /> class containing the Logging configuration.
    /// </summary>
    public Logging Logging { get; set; } = new();
}
