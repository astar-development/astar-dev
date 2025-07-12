using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Files.Api.Endpoints;

/// <summary>
/// </summary>
public static class FileContextExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="directoryName"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> WhereDirectoryNameMatches(this IQueryable<FileDetail> files, string directoryName, bool recursive)
        => recursive
               ? files.Where(fd => fd.DirectoryName.StartsWith(directoryName))
               : files.Where(fd => fd.DirectoryName == directoryName);
}
