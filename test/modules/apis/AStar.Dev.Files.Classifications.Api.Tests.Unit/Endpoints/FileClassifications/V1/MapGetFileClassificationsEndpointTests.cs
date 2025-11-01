using System.Net.Http.Json;
using System.Text.Json;
using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit.Endpoints.FileClassifications.V1;

public class MapGetFileClassificationsEndpointTests
{
    [Fact]
    public async Task Endpoint_Should_Be_Mapped_With_Expected_Metadata()
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();

        builder.Services.AddApiVersioning().AddApiExplorer();

        var dbName1 = $"fc-tests-meta-{Guid.NewGuid()}";
        builder.Services.AddDbContext<FilesContext>(o => o.UseInMemoryDatabase(dbName1));
        builder.Services.AddScoped<GetFileClassificationsHandler>();

        await using var app = builder.Build();

        app.MapFileClassificationsGetEndpoint();

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
    public async Task Endpoint_Should_Return_Data_From_Handler()
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();

        builder.Services.AddApiVersioning().AddApiExplorer();

        var dbName2 = $"fc-tests-e2e-{Guid.NewGuid()}";
        builder.Services.AddDbContext<FilesContext>(o => o.UseInMemoryDatabase(dbName2));
        builder.Services.AddScoped<GetFileClassificationsHandler>();

        await using var app = builder.Build();

        // Seed EF entities via DbContext from DI
        using(var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FilesContext>();
            db.FileClassifications.AddRange(
                new Infrastructure.FilesDb.Models.FileClassification { Id = Guid.CreateVersion7(), Name = "CHARLIE", IncludeInSearch = true, Celebrity = false },
                new Infrastructure.FilesDb.Models.FileClassification { Id = Guid.CreateVersion7(), Name = "ALPHA", IncludeInSearch = false, Celebrity = true },
                new Infrastructure.FilesDb.Models.FileClassification { Id = Guid.CreateVersion7(), Name = "BRAVO", IncludeInSearch = true, Celebrity = true }
            );
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        app.MapFileClassificationsGetEndpoint();

        await app.StartAsync(TestContext.Current.CancellationToken);
        try
        {
            var client = app.GetTestClient();

            var url = $"{EndpointConstants.FileClassificationsEndpoint}/?api-version=1.0&CurrentPage=1&ItemsPerPage=10";
            var payload = await client.GetFromJsonAsync<IReadOnlyCollection<GetFileClassificationsResponse>>(
                url,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                TestContext.Current.CancellationToken);

            payload.ShouldNotBeNull();
            payload.Count.ShouldBe(3);

            payload.Select(x => x.Name).ShouldBe(new[] { "ALPHA", "BRAVO", "CHARLIE" });

            // Spot-check some flags to ensure mapping fidelity
            var alpha = payload.First(x => x.Name == "ALPHA");
            alpha.IncludeInSearch.ShouldBeFalse();
            alpha.Celebrity.ShouldBeTrue();
        }
        finally
        {
            await app.StopAsync(TestContext.Current.CancellationToken);
        }
    }
}
