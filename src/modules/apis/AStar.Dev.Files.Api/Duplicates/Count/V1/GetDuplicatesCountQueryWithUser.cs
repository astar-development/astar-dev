using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AStar.Dev.Infrastructure.FilesDb.Models;
using SearchType = AStar.Dev.Files.Api.Config.SearchType;

namespace AStar.Dev.Files.Api.Duplicates.Count.V1;

public sealed class GetDuplicatesCountQueryWithUser(ClaimsPrincipal user, GetDuplicatesCountQuery parameters)
{
    /// <summary>
    /// </summary>
    [Required]
    public string SearchFolder => parameters.SearchFolder;

    /// <summary>
    /// </summary>
    public bool Recursive => parameters.Recursive;

    /// <summary>
    /// </summary>
    public bool ExcludeViewed => parameters.ExcludeViewed;

    /// <summary>
    /// </summary>
    public bool IncludeSoftDeleted => parameters.IncludeSoftDeleted;

    /// <summary>
    ///     What was the purpose / thought behind this?
    /// </summary>
    public bool IncludeMarkedForDeletion => parameters.IncludeMarkedForDeletion;

    /// <summary>
    /// </summary>
    public string? SearchText => parameters.SearchText;

    /// <summary>
    /// </summary>
    [Required]
    public int CurrentPage => parameters.CurrentPage;

    /// <summary>
    /// </summary>
    [Required]
    public int ItemsPerPage => parameters.ItemsPerPage;

    /// <summary>
    /// </summary>
    [Range(50, 850, ErrorMessage = "Please specify a thumbnail size between 50 and 850 pixels.")]
    public int MaximumSizeOfThumbnail => parameters.MaximumSizeOfThumbnail;

    /// <summary>
    /// </summary>
    [Range(50, 999999, ErrorMessage = "Please specify an image size between 500 and 999999 (NOT recommended!) pixels.")]
    public int MaximumSizeOfImage => parameters.MaximumSizeOfImage;

    /// <summary>
    /// </summary>
    [Required]
    public SortOrder SortOrder => parameters.SortOrder;

    /// <summary>
    /// </summary>
    public SearchType SearchType { get; } = SearchType.Duplicates;

    public ClaimsPrincipal User => user;
}
