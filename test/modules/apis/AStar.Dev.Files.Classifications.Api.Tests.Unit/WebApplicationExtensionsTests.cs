using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit;

public class WebApplicationExtensionsTests
{
    [Fact]
    public async Task UseFilesClassificationsApplicationServices_Should_Map_Endpoint_With_Expected_Metadata()
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();

        builder.Services.AddApiVersioning().AddApiExplorer();

        builder.Services.AddDbContext<FilesContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        builder.Services.AddScoped<GetFileClassificationsHandler>();

        await using var app = builder.Build();

        app.UseFilesClassificationsApplicationServices();

        await app.StartAsync(TestContext.Current.CancellationToken);

        try
        {
            var dataSource = app.Services.GetRequiredService<EndpointDataSource>();
            var routeEndpoints = dataSource.Endpoints.OfType<RouteEndpoint>().ToArray();

            var endpoint =
                routeEndpoints.FirstOrDefault(e =>
                    e.RoutePattern.RawText?.Contains(EndpointConstants.FileClassificationsEndpoint, StringComparison.Ordinal) == true)
                ?? routeEndpoints.FirstOrDefault(e =>
                    e.Metadata.GetMetadata<ITagsMetadata>()?.Tags.Contains(EndpointConstants.FileClassificationsTag) == true);

            endpoint.ShouldNotBeNull();

            var methodMetadata = endpoint.Metadata.GetMetadata<HttpMethodMetadata>();
            methodMetadata.ShouldNotBeNull();
            methodMetadata.HttpMethods.ShouldContain("GET");

            var tags = endpoint.Metadata.GetMetadata<ITagsMetadata>();
            tags.ShouldNotBeNull();
            tags.Tags.ShouldContain(EndpointConstants.FileClassificationsTag);

            var produces = endpoint.Metadata.GetOrderedMetadata<IProducesResponseTypeMetadata>().Select(m => m.StatusCode).ToArray();
            produces.ShouldContain(401);
            produces.ShouldContain(403);
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    public void UseFilesClassificationsApplicationServices_Without_ApiVersioning_Should_Not_Map_Endpoint()
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();

        using var app = builder.Build();

        app.UseFilesClassificationsApplicationServices();

        var dataSource = app.Services.GetRequiredService<EndpointDataSource>();
        var endpoint = dataSource.Endpoints
            .OfType<RouteEndpoint>()
            .FirstOrDefault(e =>
                e.RoutePattern.RawText?.Contains(EndpointConstants.FileClassificationsEndpoint, StringComparison.Ordinal) == true);

        endpoint.ShouldBeNull();
    }
}
