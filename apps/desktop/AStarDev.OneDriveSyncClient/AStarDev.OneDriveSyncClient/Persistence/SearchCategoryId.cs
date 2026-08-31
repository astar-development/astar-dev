namespace AStarDev.OneDriveSyncClient.Persistence;

public readonly record struct SearchCategoryId(Guid Value)
{
    public static implicit operator Guid(SearchCategoryId id) => id.Value;

    public static implicit operator SearchCategoryId(Guid value) => new(value);
}