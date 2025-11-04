using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text;
using AStar.Dev.Infrastructure.Data;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     The FileDetail class containing the current properties
/// </summary>
[DebuggerDisplay("FileName: {FileName.Value}, DirectoryName: {DirectoryName.Value}")]
public sealed class FileDetail : AuditableEntity
{
    /// <summary>
    ///     The default constructor required by EF Core
    /// </summary>
    public FileDetail()
    {
    }

    /// <summary>
    ///     The copy constructor that allows for passing an instance of FileInfo to this class, simplifying consumer code
    /// </summary>
    /// <param name="fileInfo">
    ///     The instance of FileInfo to map
    /// </param>
    [SetsRequiredMembers]
    public FileDetail(IFileInfo fileInfo)
    {
        FileName = new FileName(fileInfo.Name);
        DirectoryName = new DirectoryName(fileInfo.DirectoryName!);
        FileSize = fileInfo.Length;
        CreatedDate = fileInfo.CreationTimeUtc;
        UpdatedDate = fileInfo.LastWriteTimeUtc;
    }

    /// <summary>
    /// </summary>
    public ICollection<FileClassification> FileClassifications { get; set; } = new List<FileClassification>();

    /// <summary>
    ///     Gets or sets the ID of the <see href="FileDetail"></see>. I know, shocking...
    /// </summary>
    public FileId Id { get; set; } = new();

    /// <summary>
    /// </summary>
    public FileAccessDetail FileAccessDetail { get; set; } = new();

    /// <summary>
    ///     Gets or sets the file name. I know, shocking...
    /// </summary>
    public required FileName FileName { get; set; }

    /// <summary>
    ///     Gets or sets the name of the directory containing the file detail. I know, shocking...
    /// </summary>
    public required DirectoryName DirectoryName { get; set; }

    /// <summary>
    ///     Gets the full name of the file with the path combined
    /// </summary>
    public string FullNameWithPath => Path.Combine(DirectoryName.Value, FileName.Value ?? "");

    /// <summary>
    ///     Gets or sets the height of the image. I know, shocking...
    /// </summary>
    public ImageDetail ImageDetail { get; set; } = new(null, null);

    /// <summary>
    ///     Gets or sets the file size. I know, shocking...
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     Gets or sets the file Creation Date. I know, shocking...
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    ///     Gets or sets the file Updated Date. I know, shocking...
    /// </summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>
    ///     Gets or sets whether the file is of a supported image type
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    ///     Gets or sets the file handle. I know, shocking...
    /// </summary>
    public FileHandle FileHandle { get; set; }

    /// <summary>
    /// </summary>
    public DeletionStatus? DeletionStatus { get; set; }

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>
    ///     This object serialized as a JSON object.
    /// </returns>
    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        _ = sb.Append(FileName);
        _ = sb.Append(" - ");
        _ = sb.Append(FullNameWithPath);

        return sb.ToString();
    }
}
