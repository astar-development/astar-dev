namespace AStar.Dev.OneDrive.Client.Models;

public sealed class GraphFile
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long? Size { get; set; }
    public string SizeText => Size.HasValue ? $"{Size.Value} bytes" : string.Empty;
    public DateTimeOffset? LastModified { get; set; }
    public string LastModifiedText => LastModified?.ToString("g") ?? string.Empty;
}
