using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
/// Provides a method to map the `GET` API endpoint for retrieving file classifications.
/// This endpoint is designed to support versioning and applies standard response handling for retrieving file classification data.
/// </summary>
public static class MapGetFileClassificationsEndpoint
{
    /// <summary>
    /// Maps the file classifications GET API endpoint for the application.
    /// This method sets up an API endpoint to retrieve file classifications with the specified configurations
    /// including versioning, route grouping, and response handling.
    /// </summary>
    /// <param name="endpointRouteBuilder">
    /// The <see cref="IEndpointRouteBuilder"/> used to configure and add the route to the application.
    /// </param>
    /// <example>
    /// To map the endpoint in your application, use the following example:
    /// <code>
    /// var builder = WebApplication.CreateBuilder(args);
    /// var app = builder.Build();
    /// app.UseFilesClassificationsApplicationServices();
    /// app.Run();
    /// </code>
    /// </example>
    public static void MapFileClassificationsGetEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        IVersionedEndpointRouteBuilder versionedApi = endpointRouteBuilder.NewVersionedApi(EndpointConstants.GetFileClassificationsGroupName);

        RouteGroupBuilder apiGroup = versionedApi
            .MapGroup(EndpointConstants.FileClassificationsEndpoint)
            .HasApiVersion(1.0);

        _ = apiGroup.MapGet("/", async ([AsParameters] GetFileClassificationRequest fileClassifications, [FromServices] FilesContext filesContext,
                    [FromServices] GetFileClassificationsHandler handler,
                    CancellationToken cancellationToken)
                => await handler.HandleAsync(fileClassifications, filesContext, cancellationToken))
            .Produces<IReadOnlyCollection<GetFileClassificationsResponse>>()
            .Produces(401)
            .Produces(403)
            .WithName(EndpointConstants.GetFileClassificationsName)
            .WithTags(EndpointConstants.FileClassificationsTag);
    }
}
