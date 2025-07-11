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
    public virtual DbSet<FileDetail> FileDetails { get; set; } = null!;

    /// <summary>
    /// </summary>
    public DbSet<FileNamePart> FileNameParts { get;        set; } = null!;

    /// <summary>
    ///     The list of Events
    /// </summary>
    public virtual DbSet<Event> Events { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the File Classifications
    /// </summary>
    public DbSet<FileClassification> FileClassifications { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the Duplicate Details
    /// </summary>
    public DbSet<DuplicateDetail> DuplicateDetails { get; set; }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=filesDb;Trusted_Connection=True;");
    }

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
            .Entity<DuplicateDetail>(eb =>
                                     {
                                         eb.HasNoKey();
                                         eb.ToView("vw_DuplicatesDetails");
                                     });
    }
}
