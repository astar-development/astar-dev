namespace AStar.Dev.Usage.Logger.Usage.GetAll.V1;

/// <summary>
/// </summary>
/// <param name="ApiName"></param>
/// <param name="ApiEndpoint"></param>
/// <param name="HttpMethod"></param>
/// <param name="ElapsedMilliseconds"></param>
/// <param name="StatusCode"></param>
public record ApiUsageEventDto(string ApiName, string ApiEndpoint, string HttpMethod, double ElapsedMilliseconds, int StatusCode)
{
    /// <summary>
    /// </summary>
    public Guid Id { get; init; } = Guid.CreateVersion7();

    /// <summary>
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
