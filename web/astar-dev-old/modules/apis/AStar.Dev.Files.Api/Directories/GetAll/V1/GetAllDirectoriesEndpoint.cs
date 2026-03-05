using System.IO.Abstractions;
using Asp.Versioning.Builder;

namespace AStar.Dev.Files.Api.Directories.GetAll.V1;

public sealed class GetAllDirectoriesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.DirectoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.DirectoriesEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/{rootDirectory}",
                   async (string rootDirectory, IFileSystem fileSystem, HttpContext context, ILogger<GetAllDirectoriesEndpoint> logger, CancellationToken cancellationToken) =>
                       await Handle(new GetAllDirectoriesQuery(rootDirectory, context.User), fileSystem, logger, cancellationToken))
           .Produces<IEnumerable<string>>()
           .Produces(401)
           .Produces(500)
           .WithDescription("Get all scrape directories - shared across all sites")
           .WithSummary("Get all scrape directories")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.DirectoriesTag);
    }

    private static Task<IResult> Handle(GetAllDirectoriesQuery request, IFileSystem fileSystem, ILogger<GetAllDirectoriesEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting all directories starting from: {RootDirectory}", request.RootDirectory);

        IEnumerable<string> dirs = fileSystem.Directory.EnumerateDirectories(request.RootDirectory, "*", SearchOption.TopDirectoryOnly);

        logger.LogDebug("Returning all directories ({RootDirectory})", request.RootDirectory);

        return Task.FromResult<IResult>(TypedResults.Ok(dirs));
    }
}
