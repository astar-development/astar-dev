using AStarDev.LoggingOTel;

namespace AStarDev.Web.Logging;

/// <summary>
///     App-local compatibility shim bridging the old <c>AddOTelLogging()</c> call site to the current
///     <see cref="OTelLoggingConfigurator" /> API in <c>AStarDev.LoggingOTel</c>.
/// </summary>
internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddOTelLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ConfigureOTelLogging(builder.Configuration);

        return builder;
    }
}
