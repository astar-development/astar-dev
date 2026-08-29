using AStarDev.OneDriveSyncClient.Persistence;

namespace AStarDev.OneDriveSyncClient.TestsUnit;

public class UnitTest1
{
    [Fact]
    public void SearchConfigurationShouldTrackItsCategories()
    {
        var configuration = new SearchConfiguration(
            new SearchConfigurationId(Guid.NewGuid()),
            new Uri("https://example.com"),
            "prefix",
            "suffix",
            "topWallpapers",
            "subscriptions",
            1000,
            1,
            10,
            1,
            5,
            1,
            2,
            true);

        var category = new SearchCategory(
            new SearchCategoryId(Guid.NewGuid()),
            configuration.Id,
            "Nature",
            42,
            3,
            8)
        {
            SearchConfiguration = configuration
        };
        configuration.SearchCategories.Add(category);

        Assert.Contains(category, configuration.SearchCategories);
        Assert.Same(configuration, category.SearchConfiguration);
    }
}
