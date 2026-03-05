using System.Security.Claims;

namespace AStar.Dev.Files.Api.Directories.GetAll.V1;

/// <summary>
///     The <see cref="GetAllDirectoriesQuery" /> class is used to define the All Scrape Directories Query.
/// </summary>
/// <param name="rootDirectory"></param>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
public sealed class GetAllDirectoriesQuery(string rootDirectory, ClaimsPrincipal user)
{
    /// <summary>
    /// </summary>
    public ClaimsPrincipal User { get; } = user;

    /// <summary>
    /// </summary>
    public string RootDirectory { get; } = rootDirectory;
}
