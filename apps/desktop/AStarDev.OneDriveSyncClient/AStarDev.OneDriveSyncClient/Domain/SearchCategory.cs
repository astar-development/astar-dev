namespace AStarDev.OneDriveSyncClient.Domain;

public record SearchCategory(SearchCategoryId Id, SearchConfigurationId SearchConfigurationId, string Name, int LastKnownImageCount, int LastPageVisited, int TotalPages);

