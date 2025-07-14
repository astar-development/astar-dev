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
    public static IQueryable<FileDetail> WhereDirectoryNameMatches(this IQueryable<FileDetail> files, string directoryName, bool recursive)
        => recursive
               ? files.Where(fileDetail => fileDetail.DirectoryName.StartsWith(directoryName))
               : files.Where(fileDetail => fileDetail.DirectoryName == directoryName);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="directoryName"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> WhereDirectoryNameMatches(this IQueryable<DuplicateDetail> files, string directoryName, bool recursive)
        => recursive
               ? files.Where(fileDetail => fileDetail.DirectoryName.StartsWith(directoryName))
               : files.Where(fileDetail => fileDetail.DirectoryName == directoryName);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="excludeViewedWithinDays"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> ExcludeViewed(this IQueryable<FileDetail> files, int excludeViewedWithinDays)
        => excludeViewedWithinDays == 0
               ? files
               : files.Where(fileDetail => fileDetail.FileLastViewed < DateTimeOffset.UtcNow.AddDays(-excludeViewedWithinDays));

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="excludeViewedWithinDays"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> ExcludeViewed(this IQueryable<DuplicateDetail> files, int excludeViewedWithinDays)
        => excludeViewedWithinDays == 0
               ? files
               : files.Where(fileDetail => fileDetail.FileLastViewed < DateTimeOffset.UtcNow.AddDays(-excludeViewedWithinDays));

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="includeMarkedForDeletion"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> IncludeMarkedForDeletion(this IQueryable<FileDetail> files, bool includeMarkedForDeletion)
        => includeMarkedForDeletion
               ? files
               : files.Where(fileDetail => fileDetail.DeletionStatus.HardDeletePending == null && fileDetail.DeletionStatus.SoftDeleted == null && fileDetail.DeletionStatus.SoftDeletePending == null);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="includeMarkedForDeletion"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> IncludeMarkedForDeletion(this IQueryable<DuplicateDetail> files, bool includeMarkedForDeletion)
        => includeMarkedForDeletion
               ? files
               : files.Where(fileDetail => fileDetail.HardDeletePending == null && fileDetail.SoftDeleted == null && fileDetail.SoftDeletePending == null);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> SelectFilesMatching(this IQueryable<FileDetail> files, string? searchText)
        => string.IsNullOrEmpty(searchText)
               ? files
               : files.Where(fileDetail => fileDetail.DirectoryName.Contains(searchText) || fileDetail.FileName.Contains(searchText));

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchText"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> SelectFilesMatching(this IQueryable<DuplicateDetail> files, string? searchText)
        => string.IsNullOrEmpty(searchText)
               ? files
               : files.Where(fileDetail => fileDetail.DirectoryName.Contains(searchText) || fileDetail.FileName.Contains(searchText));

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="currentPage"></param>
    /// <param name="itemsPerPage"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> SelectRequestedPage(this IQueryable<FileDetail> files, int currentPage, int itemsPerPage)
        => files.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="currentPage"></param>
    /// <param name="itemsPerPage"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> SelectRequestedPage(this IQueryable<DuplicateDetail> files, int currentPage, int itemsPerPage)
        => files.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> OrderAsRequested(this IQueryable<FileDetail> files, SortOrder sortOrder) =>
        sortOrder switch
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
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> OrderAsRequested(this IQueryable<DuplicateDetail> files, SortOrder sortOrder) =>
        sortOrder switch
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
    public static IQueryable<FileDetail> SetSearchType(this IQueryable<FileDetail> files, SearchType searchType) =>
        searchType switch
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
    public static IQueryable<DuplicateDetail> SetSearchType(this IQueryable<DuplicateDetail> files, SearchType searchType) =>
        searchType switch
        {
            SearchType.DuplicateImages => files.OrderByDescending(fileDetail => fileDetail.FileName),
            SearchType.Duplicates      => files.OrderBy(fileDetail => fileDetail.FileSize),
            _                          => throw new UnreachableException($"Invalid search type specified: {searchType}")
        };

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static IQueryable<FileDetail> SelectByFileType(this IQueryable<FileDetail> files, SearchType searchType)
        => searchType == SearchType.Images
               ? files.Where(fileDetail => fileDetail.IsImage)
               : files;

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static IQueryable<DuplicateDetail> SelectByFileType(this IQueryable<DuplicateDetail> files, SearchType searchType)
        => searchType == SearchType.Images
               ? files.Where(fileDetail => fileDetail.IsImage)
               : files;
}
