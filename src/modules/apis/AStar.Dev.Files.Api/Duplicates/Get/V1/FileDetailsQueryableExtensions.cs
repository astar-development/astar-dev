using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;
using SearchType = AStar.Dev.Files.Api.Config.SearchType;

namespace AStar.Dev.Files.Api.Duplicates.Get.V1;

public static class FileDetailsQueryableExtensions
{
    public static IQueryable<FileDetail> FilterByDirectory(this IQueryable<FileDetail> fileDetails, string searchFolder, bool recursive)
        => recursive
               ? fileDetails.Where(fileDetail => fileDetail.DirectoryName.Value.StartsWith(searchFolder))
               : fileDetails.Where(fileDetail => fileDetail.DirectoryName.Equals(searchFolder));

    public static IQueryable<DuplicatesDetails> FilterByDirectory(this IQueryable<DuplicatesDetails> duplicates, string searchFolder, bool recursive)
        => recursive
               ? duplicates.Where(duplicatesDetails => duplicatesDetails.DirectoryName.StartsWith(searchFolder))
               : duplicates.Where(duplicatesDetails => duplicatesDetails.DirectoryName.Equals(searchFolder));

    public static IQueryable<FileDetail> ExcludeAllButImages(this IQueryable<FileDetail> fileDetails, SearchType searchType)
        => searchType == SearchType.DuplicateImages
               ? fileDetails.Where(fileDetail => fileDetail.IsImage)
               : fileDetails;

    public static IQueryable<DuplicatesDetails> ExcludeAllButImages(this IQueryable<DuplicatesDetails> fileDetails, SearchType searchType)
        => searchType == SearchType.DuplicateImages
               ? fileDetails.Where(fileDetail => fileDetail.IsImage)
               : fileDetails;

    public static IQueryable<FileDetail> IncludeSoftDeleted(this IQueryable<FileDetail> fileDetails, bool includeSoftDeleted)
        => includeSoftDeleted
               ? fileDetails
               : fileDetails.Where(fileDetail => fileDetail.DeletionStatus.SoftDeleted != null && fileDetail.DeletionStatus.SoftDeletePending != null);

    public static IQueryable<DuplicatesDetails> IncludeSoftDeleted(this IQueryable<DuplicatesDetails> fileDetails, bool includeSoftDeleted)
        => includeSoftDeleted
               ? fileDetails
               : fileDetails.Where(fileDetail => !fileDetail.SoftDeleted && !fileDetail.SoftDeletePending);

    public static IQueryable<FileDetail> ExcludeViewed(this IQueryable<FileDetail> fileDetails, bool excludedViewed)
        => excludedViewed
               ? fileDetails.Where(fileDetail => fileDetail.FileAccessDetail.LastViewed == null)
               : fileDetails;

    public static IQueryable<DuplicatesDetails> ExcludeViewed(this IQueryable<DuplicatesDetails> fileDetails, bool excludedViewed)
        => excludedViewed
               ? fileDetails.Where(fileDetail => fileDetail.LastViewed == null)
               : fileDetails;

    public static IQueryable<FileDetail> IncludeWhenContains(this IQueryable<FileDetail> fileDetails, string? searchText)
        => searchText.IsNullOrWhiteSpace()
               ? fileDetails
               : fileDetails.Where(fileDetail => fileDetail.DirectoryName.Value.Contains(searchText!) || fileDetail.FileName.Value.Contains(searchText!));

    public static IQueryable<DuplicatesDetails> IncludeWhenContains(this IQueryable<DuplicatesDetails> fileDetails, string? searchText)
        => searchText.IsNullOrWhiteSpace()
               ? fileDetails
               : fileDetails.Where(fileDetail => fileDetail.DirectoryName.Contains(searchText!) || fileDetail.FileName.Contains(searchText!));

    public static IQueryable<FileDetail> FilterBySearchType(this IQueryable<FileDetail> fileDetails, SearchType searchType)
        => searchType switch
           {
               SearchType.Images => fileDetails.Where(fileDetail => fileDetail.FileName.Value.EndsWith("jpg") || fileDetail.FileName.Value.EndsWith("jpeg") ||
                                                                    fileDetail.FileName.Value.EndsWith("add remaining or access the IsImage property")),
               SearchType.All => fileDetails,
               _              => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null),
           };

    /// <summary>
    ///     Gets the count of duplicates, grouped by Size, Height and Width.
    /// </summary>
    /// <param name="files">
    ///     The files to return grouped together.
    /// </param>
    /// <returns>
    /// </returns>
    public static IEnumerable<IGrouping<FileSize, FileDetail>> FilterDuplicates(this IEnumerable<FileDetail> files)
        => files
          .GroupBy(file => FileSize.Create(file.FileSize, file.ImageDetail?.Height, file.ImageDetail?.Width),
                   new FileSizeEqualityComparer()).Where(fileGroups => fileGroups.Count() > 1);

    // public static IEnumerable<IGrouping<FileSize, FileDetail>> FilterDuplicates(this IQueryable<FileDetail> fileDetails) =>
    //     fileDetails.GroupBy(file => FileSize.Create(file.FileSize, file.Height, file.Width),
    //                         new FileSizeEqualityComparer()).Where(files => files.Count() > 1);

    public static IEnumerable<FileDetail> SelectFileDetailPage(this IEnumerable<FileDetail> fileDetails, int currentPage, int itemsPerPage)
        => fileDetails.Skip((currentPage - 1) * itemsPerPage).Take(itemsPerPage);

    public static IEnumerable<FileDetail> SortBy(this IQueryable<FileDetail> fileDetails, SortOrder order)
        => order switch
           {
               SortOrder.SizeDescending => fileDetails.OrderByDescending(fileDetail => fileDetail.FileSize)
                                                      .ThenByDescending(fileDetail => fileDetail.FileName),
               SortOrder.SizeAscending => fileDetails.OrderBy(fileDetail => fileDetail.FileSize)
                                                     .ThenByDescending(fileDetail => fileDetail.FileName),
               SortOrder.NameDescending => fileDetails.OrderByDescending(fileDetail => fileDetail.DirectoryName)
                                                      .ThenByDescending(fileDetail => fileDetail.FileName),
               SortOrder.NameAscending => fileDetails.OrderBy(fileDetail => fileDetail.DirectoryName)
                                                     .ThenBy(fileDetail => fileDetail.FileName),
               _ => throw new ArgumentOutOfRangeException(nameof(order), order, null),
           };

    public static IEnumerable<DuplicatesDetails> SortBy(this IQueryable<DuplicatesDetails> fileDetails, SortOrder order)
        => order switch
           {
               SortOrder.SizeDescending => fileDetails.OrderByDescending(fileDetail => fileDetail.FileSize)
                                                      .ThenByDescending(fileDetail => fileDetail.FileName),
               SortOrder.SizeAscending => fileDetails.OrderBy(fileDetail => fileDetail.FileSize)
                                                     .ThenByDescending(fileDetail => fileDetail.FileName),
               SortOrder.NameDescending => fileDetails.OrderByDescending(fileDetail => fileDetail.DirectoryName)
                                                      .ThenByDescending(fileDetail => fileDetail.FileName),
               SortOrder.NameAscending => fileDetails.OrderBy(fileDetail => fileDetail.DirectoryName)
                                                     .ThenBy(fileDetail => fileDetail.FileName),
               _ => throw new ArgumentOutOfRangeException(nameof(order), order, null),
           };
}
