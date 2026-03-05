using System.ComponentModel.DataAnnotations;
using System.Text;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
///     The <see href="SearchParameters"></see> class
/// </summary>
public sealed class SearchParameters
{
    /// <summary>
    ///     Gets or sets the Search Folder to be used as the root for the search
    /// </summary>
    [Required]
    public string SearchFolder { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Search Type for the search
    /// </summary>
    public SearchType SearchType { get; set; }

    /// <summary>
    ///     Gets or sets whether the search is to be recursive or not
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    ///     Gets or sets whether the search includes soft-deleted or not
    /// </summary>
    public bool IncludeSoftDeleted { get; set; }

    /// <summary>
    ///     Gets or sets whether the search includes files marked for deletion or not
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    ///     Gets or sets the Current page for the search. This is not validated against the collection it will be applied to
    /// </summary>
    [Required]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the maximum items per page for the search. This parameter can be set between 5 and 50. The default is 10
    /// </summary>
    [Range(5, 50, ErrorMessage = "Please specify the number of items per page between 5 and 50.")]
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    ///     Gets or sets the maximum items per page for the search. This parameter can be set between 50 and 850. The default is 850
    /// </summary>
    [Range(50, 850, ErrorMessage = "Please specify a thumbnail size between 50 and 850 pixels.")]
    public int MaximumSizeOfThumbnail { get; set; } = 850;

    /// <summary>
    ///     Gets or sets the maximum size of the image to be returned by the search. This parameter can be set between 50 and 999999. The default is 1500
    /// </summary>
    [Range(50, 999999, ErrorMessage = "Please specify an image size between 500 and 999999 (NOT recommended!) pixels.")]
    public int MaximumSizeOfImage { get; set; } = 1500;

    /// <summary>
    ///     Gets or sets the Search Order for the search. The default is by Size Descending
    /// </summary>
    [Required]
    public SortOrder SortOrder { get; set; } = SortOrder.SizeDescending;

    /// <summary>
    ///     Gets or sets the Search Text for the search. If no search text is supplied, the results will not be filtered based
    ///     on any containing text. Yep, shocking...
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    ///     Gets or sets the excluded view settings for the search
    /// </summary>
    public ExcludedViewSettings ExcludedViewSettings { get; set; } = new();

    /// <summary>
    ///     This method builds a string ready to be passed as the query string for calls to the Files API itself
    /// </summary>
    /// <returns>A string ready to be passed as the query string</returns>
    public string ToQueryString()
    {
        var sb = new StringBuilder();
        _ = sb.Append($"{nameof(SearchFolder)}={SearchFolder}");
        _ = sb.Append($"&{nameof(CurrentPage)}={CurrentPage}");
        _ = sb.Append($"&{nameof(ItemsPerPage)}={ItemsPerPage}");
        _ = sb.Append($"&{nameof(SearchType)}={SearchType}");
        _ = sb.Append($"&{nameof(Recursive)}={Recursive}");
        _ = sb.Append($"&{nameof(IncludeSoftDeleted)}={IncludeSoftDeleted}");
        _ = sb.Append($"&{nameof(IncludeMarkedForDeletion)}={IncludeMarkedForDeletion}");
        _ = sb.Append($"&{nameof(ExcludedViewSettings.ExcludeViewed)}={ExcludedViewSettings.ExcludeViewed}");
        _ = sb.Append($"&{nameof(SortOrder)}={SortOrder}");
        _ = sb.Append($"&{nameof(MaximumSizeOfThumbnail)}={MaximumSizeOfThumbnail}");
        _ = sb.Append($"&{nameof(MaximumSizeOfImage)}={MaximumSizeOfImage}");

        _ = sb.Append(
                      $"&{nameof(ExcludedViewSettings.ExcludeViewedPeriodInDays)}={ExcludedViewSettings.ExcludeViewedPeriodInDays}");

        _ = sb.Append($"&{nameof(SearchText)}={SearchText}");

        return sb.ToString();
    }

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>This object serialized as a JSON object</returns>
    public override string ToString()
        => this.ToJson();
}
