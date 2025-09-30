namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
///     The <see cref="AddFilesResponse" /> contains the detail of the file being added.
///     The name is pluralised as we actually return a list and the endpoint is pluralised...
/// </summary>
public class AddFilesResponse
{
    /// <summary>
    ///     Gets or sets the ID of the <see href="FileDetail"></see>. I know, shocking...
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// </summary>
    public ImageDetails ImageDetails { get; set; } = new();

    /// <summary>
    ///     Gets or sets the file name. I know, shocking...
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the directory containing the file detail. I know, shocking...
    /// </summary>
    public string DirectoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets the full name of the file with the path combined
    /// </summary>
    public string FullNameWithPath => Path.Combine(DirectoryName, FileName);

    /// <summary>
    ///     Gets or sets the file size. I know, shocking...
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     Gets or sets whether the file is of a supported image type
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    ///     Gets or sets the file handle. I know, shocking...
    /// </summary>
    public string FileHandle { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset FileLastModified { get; set; }
}
