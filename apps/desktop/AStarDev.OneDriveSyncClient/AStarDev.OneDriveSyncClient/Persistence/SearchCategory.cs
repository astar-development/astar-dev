namespace AStarDev.OneDriveSyncClient.Persistence;

public class SearchCategory
{
    public SearchCategory(SearchCategoryId id, SearchConfigurationId searchConfigurationId, string name, int lastKnownImageCount, int lastPageVisited, int totalPages)
    {
        this.Id = id;
        this.SearchConfigurationId = searchConfigurationId;
        this.Name = name;
        this.LastKnownImageCount = lastKnownImageCount;
        this.LastPageVisited = lastPageVisited;
        this.TotalPages = totalPages;
    }

    public SearchCategoryId Id { get; init; }

    public SearchConfigurationId SearchConfigurationId { get; set; }

    public SearchConfiguration SearchConfiguration { get; set; } = default!;

    public string Name { get; init; }

    public int LastKnownImageCount { get; init; }

    public int LastPageVisited { get; init; }

    public int TotalPages { get; init; }
}