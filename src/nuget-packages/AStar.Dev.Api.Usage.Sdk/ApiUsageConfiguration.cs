namespace AStar.Dev.Api.Usage.Sdk;

/// <summary>
/// </summary>
public sealed class ApiUsageConfiguration
{
    /// <summary>
    /// </summary>
    public static string ConfigurationSectionName => "ApiUsageConfiguration";

    /// <summary></summary>
    public required string HostName { get; init; }

    /// <summary></summary>
    public required string UserName { get; init; }

    /// <summary></summary>
    public required string Password { get; init; }

    /// <summary></summary>
    public required string QueueName { get; init; }
}
