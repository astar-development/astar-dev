using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Web.Models;

/// <summary>
/// </summary>
public class SearchModel
{
    /// <summary>
    ///     Gets or sets the starting directory for the search.
    /// </summary>
    [Required]
    public string StartingDirectory { get; set; } = "/home/jason/Documents/Pictures/_lookat";

    /// <summary>
    ///     Gets or sets the text to search for within the filename or path.
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to search subdirectories.
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    ///     Gets or sets the exclude files viewed with the past N days. 0 should be used to include viewed
    /// </summary>
    public string ExcludeViewedWithinDays { get; set; } = "0";

    /// <summary>
    ///     Gets or sets a value indicating whether to include files marked for deletion in the search.
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    // ReSharper disable once UseNameOfInsteadOfToString - breaks the UI mappings and submission
    public string SearchType { get; set; } = Models.SearchType.DuplicateImages.ToString();

    /// <summary>
    ///     Gets or sets the options for sorting the search results.
    /// </summary>
    // ReSharper disable once UseNameOfInsteadOfToString - breaks the UI mappings and submission
    public string SortOrder { get; set; } = Models.SortOrder.SizeDescending.ToString();

    /// <summary>
    ///     Gets or sets the options for excluding viewed files within a certain number of days.
    /// </summary>
    public IEnumerable<int> ExcludeViewedWithinDaysOptions { get; } = [0, 1, 3, 7, 14, 30];

    /// <summary>
    ///     Gets or sets the primary classification label for the file.
    /// </summary>
    public string? FileClassification1 { get; set; }

    /// <summary>
    ///     Gets or sets the secondary classification label for the file.
    /// </summary>
    public string? FileClassification2 { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to use AND operator between multiple classification labels.
    /// </summary>
    public bool UseAndOperator { get; set; } = true;
}
