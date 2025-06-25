using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Infrastructure.AdminDb.SeedData;
using AStar.Dev.Infrastructure.Data;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Infrastructure.AdminDb;

/// <summary>
///     The AdminContext class
/// </summary>
/// <remarks>
///     The various classes that make up the admin context
/// </remarks>
public sealed class AdminContext : DbContext
{
    private readonly AStarDbContextOptions astarDbContextOptions;
    private readonly ConnectionString      connectionString;

    /// <summary>
    ///     Alternative constructor used when creating migrations, the connection string is hard-coded
    /// </summary>
    [Refactor(1, 1, "Remove the hard-coded connection string")]
    public AdminContext()
        : this(
               new() { Value = "Data Source=sql1;Initial Catalog=AdminDb;Trusted_Connection=True;TrustServerCertificate=true", },
               new())
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="connectionString">
    /// </param>
    /// <param name="astarDbContextOptions">
    /// </param>
    public AdminContext(ConnectionString connectionString, AStarDbContextOptions astarDbContextOptions)
    {
        this.connectionString      = connectionString;
        this.astarDbContextOptions = astarDbContextOptions;
    }

    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="NotImplementedException"></exception>
    public AdminContext(DbContextOptions<AdminContext> options)
        : base(options)
    {
        astarDbContextOptions = new();
        connectionString      = new();
    }

    /// <summary>
    ///     ScrapeDirectories
    /// </summary>
    public DbSet<ScrapeDirectory> ScrapeDirectories { get; set; } = null!;

    /// <summary>
    ///     SearchCategory
    /// </summary>
    public DbSet<SearchCategory> SearchCategory { get; set; } = null!;

    /// <summary>
    ///     SearchConfiguration
    /// </summary>
    public DbSet<SearchConfiguration> SearchConfiguration { get; set; } = null!;

    /// <summary>
    ///     SiteConfigurations
    /// </summary>
    public DbSet<SiteConfiguration> SiteConfiguration { get; set; } = null!;

    /// <summary>
    ///     The overridden OnModelCreating method.
    /// </summary>
    /// <param name="modelBuilder">
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
        _ = modelBuilder.HasDefaultSchema("admin");
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminContext).Assembly);
    }

    /// <summary>
    ///     The overridden OnConfiguring method.
    /// </summary>
    /// <param name="optionsBuilder">
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        _ = optionsBuilder.UseSqlServer(connectionString.Value);

        optionsBuilder
           .UseSeeding((context, _) =>
                       {
                           SeedData(context);

                           context.SaveChanges();
                       })
           .UseAsyncSeeding(async (context, _, cancellationToken) =>
                            {
                                SeedData(context);

                                await context.SaveChangesAsync(cancellationToken);
                            });

        if (astarDbContextOptions.EnableLogging)
        {
            _ = optionsBuilder.UseLoggerFactory(CreateLoggerFactory())
                              .LogTo(Console.WriteLine);

            if (astarDbContextOptions.IncludeSensitiveData)
            {
                _ = optionsBuilder.EnableSensitiveDataLogging();
            }

            return;
        }

        _ = optionsBuilder.UseLoggerFactory(CreateEmptyLoggerFactory());
    }

    private static void SeedData(DbContext context)
    {
        ScrapeDirectoryData.Seed(context);
        SearchCategoryData.Seed(context);
        SearchConfigurationData.Seed(context);
        SiteConfigurationData.Seed(context);
    }

    private static ILoggerFactory CreateEmptyLoggerFactory() =>
        LoggerFactory.Create(builder => builder.AddFilter((_, _) => false));

    private static ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(static builder => builder
                                .AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information));
}
