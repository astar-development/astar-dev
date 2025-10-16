using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Web.Models;

public class SearchModel
{
    /// <summary>
    /// </summary>
    [Required]
    public string StartingDirectory { get; set; } = "/media/jason/ONEDRIVE/Pictures/_lookat";

    /// <summary>
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    ///     Gets or sets the exclude files viewed with the past N days. 0 should be used to include viewed
    /// </summary>
    public string ExcludeViewedWithinDays { get; set; } = "0";

    /// <summary>
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    public string SearchType { get; set; } = Models.SearchType.Duplicates.ToString();

    /// <summary>
    /// </summary>
    public SortOrder SortOrder { get; set; }

    /// <summary>
    /// </summary>
    public IEnumerable<int> ExcludeViewedWithinDaysOptions { get; } = [0, 1, 3, 7, 14, 30];
}
