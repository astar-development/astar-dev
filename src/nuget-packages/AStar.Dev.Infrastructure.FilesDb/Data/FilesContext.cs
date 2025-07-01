using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.FilesDb.Data;

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

    /// <summary>
    ///     The list of file access details in the dB
    /// </summary>
    public DbSet<ImageDetails> FileAccessDetails { get; set; } = null!;

    /// <summary>
    /// </summary>
    public DbSet<FileNamePart> FileNameParts { get;        set; } = null!;

    /// <summary>
    ///     The list of Events
    /// </summary>
    public DbSet<Events> Events { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the File Classifications
    /// </summary>
    public DbSet<FileClassification> FileClassifications { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the DuplicatesDetails loaded from the configured view in the database
    /// </summary>
    public DbSet<DuplicatesDetails> DuplicatesDetails { get; set; } = null!;

    // /// <inheritdoc />
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     base.OnConfiguring(optionsBuilder);
    //     optionsBuilder.UseSqlServer("Data Source=localhost,nnnnn;Initial Catalog=FilesDb;User ID=sa;Password=<SomeSecurePasswordHere1!>;TrustServerCertificate=True");
    // }

    /// <summary>
    ///     The overridden OnModelCreating method
    /// </summary>
    /// <param name="modelBuilder">
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = modelBuilder.HasDefaultSchema(Constants.SchemaName);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(FilesContext).Assembly);

        modelBuilder
            .Entity<DuplicatesDetails>(eb =>
                                       {
                                           eb.HasNoKey();
                                           eb.ToView("vw_DuplicatesDetails");
                                       });
    }
}
