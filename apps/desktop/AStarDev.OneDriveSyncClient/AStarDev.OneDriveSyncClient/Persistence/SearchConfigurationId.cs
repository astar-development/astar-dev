using System;

namespace AStarDev.OneDriveSyncClient.Persistence;

public readonly record struct SearchConfigurationId(Guid Value)
{
    public static implicit operator Guid(SearchConfigurationId id) => id.Value;

    public static implicit operator SearchConfigurationId(Guid value) => new(value);
}