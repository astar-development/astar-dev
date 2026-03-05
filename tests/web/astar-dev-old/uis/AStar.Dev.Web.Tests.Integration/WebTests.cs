using Aspire.Hosting;
using Microsoft.Extensions.Logging;
using Projects;

namespace AStar.Dev.Web.Tests.Integration;

public sealed class WebTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        CancellationToken cancellationToken = new CancellationTokenSource(DefaultTimeout).Token;

        IDistributedApplicationTestingBuilder appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<AStar_Dev_Web_AppHost>(cancellationToken);
        _ = appHost.Services.AddLogging(logging =>
        {
            _ = logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            _ = logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            _ = logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });
        _ = appHost.Services.ConfigureHttpClientDefaults(clientBuilder => _ = clientBuilder.AddStandardResilienceHandler());

        await using DistributedApplication app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        HttpClient httpClient = app.CreateHttpClient("webfrontend");
        _ = await app.ResourceNotifications.WaitForResourceHealthyAsync("webfrontend", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        HttpResponseMessage response = await httpClient.GetAsync("/", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
