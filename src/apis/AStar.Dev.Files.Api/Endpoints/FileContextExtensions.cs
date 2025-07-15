using System.Diagnostics;
using AStar.Dev.Files.Api.Endpoints.Get.V1;
using AStar.Dev.Infrastructure.FilesDb.Models;
using SortOrder = AStar.Dev.Files.Api.Endpoints.Get.V1.SortOrder;

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
    public static IQueryable<T> WhereDirectoryNameMatches<T>(this IQueryable<T> files, string directoryName, bool recursive) where T : IFileDetail
        => recursive
               ? files.Where(fileDetail => fileDetail.DirectoryName.StartsWith(directoryName))
               : files.Where(fileDetail => fileDetail.DirectoryName == directoryName);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="excludeViewedWithinDays"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IQueryable<T> ExcludeViewed<T>(this IQueryable<T> files, int excludeViewedWithinDays, TimeProvider time) where T : IFileDetail
        => excludeViewedWithinDays == 0
               ? files
               : files.Where(fileDetail => fileDetail.FileLastViewed < time.GetUtcNow().AddDays(-excludeViewedWithinDays));

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="includeMarkedForDeletion"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> IncludeMarkedForDeletion(this IQueryable<FileDetail> files, bool includeMarkedForDeletion)
        => includeMarkedForDeletion
               ? files
               : files.Where(fileDetail => fileDetail.DeletionStatus.HardDeletePending == null && fileDetail.DeletionStatus.SoftDeleted == null && fileDetail.DeletionStatus.SoftDeletePending == null);

    // /// <summary>
    // /// </summary>
    // /// <param name="files"></param>
    // /// <param name="includeMarkedForDeletion"></param>
    // /// <returns></returns>
    // public static IQueryable<DuplicateDetail> IncludeMarkedForDeletion(this IQueryable<DuplicateDetail> files, bool includeMarkedForDeletion)
    //     => includeMarkedForDeletion
    //            ? files
    //            : files.Where(fileDetail => fileDetail.HardDeletePending == null && fileDetail.SoftDeleted == null && fileDetail.SoftDeletePending == null);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public static IQueryable<T> SelectFilesMatching<T>(this IQueryable<T> files, string? searchText) where T : IFileDetail
        => string.IsNullOrEmpty(searchText)
               ? files
               : files.Where(fileDetail => fileDetail.DirectoryName.Contains(searchText) || fileDetail.FileName.Contains(searchText));

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="currentPage"></param>
    /// <param name="itemsPerPage"></param>
    /// <returns></returns>
    public static IQueryable<T> SelectRequestedPage<T>(this IQueryable<T> files, int currentPage, int itemsPerPage) where T : IFileDetail
        => files.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    public static IQueryable<T> OrderAsRequested<T>(this IQueryable<T> files, SortOrder sortOrder) where T : IFileDetail
        => sortOrder switch
           {
               SortOrder.NameAscending  => files.OrderBy(fileDetail => fileDetail.FileName),
               SortOrder.NameDescending => files.OrderByDescending(fileDetail => fileDetail.FileName),
               SortOrder.SizeAscending  => files.OrderBy(fileDetail => fileDetail.FileSize),
               SortOrder.SizeDescending => files.OrderByDescending(fileDetail => fileDetail.FileSize),
               _                        => throw new UnreachableException($"Invalid sort order specified: {sortOrder}")
           };

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static IQueryable<T> SetSearchType<T>(this IQueryable<T> files, SearchType searchType) where T : IFileDetail
        => searchType switch
           {
               SearchType.All             => files,
               SearchType.DuplicateImages => files.OrderByDescending(fileDetail => fileDetail.FileName),
               SearchType.Duplicates      => files.OrderBy(fileDetail => fileDetail.FileSize),
               SearchType.Images          => files.OrderByDescending(fileDetail => fileDetail.FileSize),
               _                          => throw new UnreachableException($"Invalid search type specified: {searchType}")
           };

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static IQueryable<T> SelectByFileType<T>(this IQueryable<T> files, SearchType searchType) where T : IFileDetail
        => searchType == SearchType.Images
               ? files.Where(fileDetail => fileDetail.IsImage)
               : files;
}
