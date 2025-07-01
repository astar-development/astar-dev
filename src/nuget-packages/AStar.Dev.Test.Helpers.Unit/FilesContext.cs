using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Test.Helpers.Unit;

/// <summary>
///     The <see cref="FilesContext" /> class
/// </summary>
/// <remarks>
///     The list of files in the dB
/// </remarks>
public class FilesContext : DbContext
{
    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    public FilesContext(DbContextOptions<FilesContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// </summary>
    public FilesContext()
        : base(new DbContextOptions<FilesContext>() )
    {
    }

    /// <summary>
    ///     The list of files in the dB
    /// </summary>
    public virtual DbSet<FileDetail> Files { get; set; } = null!;
}
