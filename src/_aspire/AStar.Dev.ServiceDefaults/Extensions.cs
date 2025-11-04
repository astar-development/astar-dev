using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace AStar.Dev.ServiceDefaults;

public static class Extensions
{
    private const string HealthEndpointPath    = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        _ = builder.ConfigureOpenTelemetry();

        _ = builder.AddDefaultHealthChecks();

        _ = builder.Services.AddServiceDiscovery();

        _ = builder.Services.ConfigureHttpClientDefaults(http =>
                                                     {
                                                         // Turn on resilience by default
                                                         _ = http.AddStandardResilienceHandler();

                                                         // Turn on service discovery by default
                                                         _ = http.AddServiceDiscovery();
                                                     });

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        _ = builder.Logging.AddOpenTelemetry(logging =>
                                         {
                                             logging.IncludeFormattedMessage = true;
                                             logging.IncludeScopes = true;
                                         });

        _ = builder.Services.AddOpenTelemetry()
               .WithMetrics(metrics =>
                            {
                                _ = metrics.AddAspNetCoreInstrumentation()
                                       .AddHttpClientInstrumentation()
                                       .AddRuntimeInstrumentation();

                                _ = metrics.AddMeter("FileScanner", "DatabaseWriter");
                            })
               .WithTracing(tracing =>
                            {
                                _ = tracing.AddSource(builder.Environment.ApplicationName)
                                    .AddAspNetCoreInstrumentation(tracing => tracing.Filter = context => !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                                                                                                         && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                                                                    )
                                       .AddHttpClientInstrumentation();

                                _ = tracing.AddSource("FileScanner", "DatabaseWriter");
                            });

        _ = builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var otlpEndpoint                                               = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!;
        if(ServiceDefaultsLogic.ShouldUseOtlpExporter(otlpEndpoint)) _ = builder.Services.AddOpenTelemetry().UseOtlpExporter();

        return builder;
    }

    /// <summary>
    ///     Extracted pure logic for ServiceDefaults extensions.
    /// </summary>
    public static class ServiceDefaultsLogic
    {
        public static bool ShouldUseOtlpExporter(string otlpEndpoint) => !string.IsNullOrWhiteSpace(otlpEndpoint);
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        _ = builder.Services.AddHealthChecks()
               .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        if(!app.Environment.IsDevelopment()) return app;

        _ = app.MapHealthChecks(HealthEndpointPath);

        _ = app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });

        return app;
    }
}
