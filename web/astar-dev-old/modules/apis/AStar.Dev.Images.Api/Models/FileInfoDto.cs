namespace AStar.Dev.Images.Api.Models;

/// <summary>
/// </summary>
public sealed class FileInfoDto
{
    /// <summary>
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public long Height { get; set; }

    /// <summary>
    /// </summary>
    public long Width { get; set; }

    /// <summary>
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// </summary>
    public string Extension
    {
        get
        {
            var extensionIndex = Name.LastIndexOf('.') + 1;

            return Name[extensionIndex..];
        }
    }
}
