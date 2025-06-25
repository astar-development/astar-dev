using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace AStar.Dev.Logging.Extensions;

/// <summary>
/// </summary>
internal static class SerilogConfigure
{
    /// <summary>
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <param name="configuration"></param>
    /// <param name="telemetryConfiguration"></param>
    /// <returns></returns>
    public static LoggerConfiguration Configure(this LoggerConfiguration loggerConfiguration, IConfiguration configuration, TelemetryConfiguration telemetryConfiguration) =>
        loggerConfiguration
           .WriteTo.ApplicationInsights(telemetryConfiguration,
                                        TelemetryConverter.Traces).WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
           .ReadFrom.Configuration(configuration);
}
