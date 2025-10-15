namespace AStar.Dev.Web.Models;

public class SearchModel
{
    /// <summary>
    /// </summary>
    public string? SearchString { get; set; }

    /// <summary>
    /// </summary>
    public string StartingDirectory { get; set; } = "/media/jason/ONEDRIVE/Pictures/_lookat";

    /// <summary>
    /// </summary>
    public SearchType SearchType { get; set; } = SearchType.All;

    /// <summary>
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    ///     Gets or sets the exclude files viewed with the past N days. 0 is used to include viewed
    /// </summary>
    public int ExcludeViewedWithinDays { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// </summary>
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    /// </summary>
    public SortOrder SortOrder { get; set; }

}
