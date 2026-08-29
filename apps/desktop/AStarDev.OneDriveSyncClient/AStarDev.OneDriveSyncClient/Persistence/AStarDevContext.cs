using Microsoft.EntityFrameworkCore;

namespace AStarDev.OneDriveSyncClient.Persistence;

public class AStarDevContext(DbContextOptions<AStarDevContext> options) : DbContext(options)
{
    public DbSet<SearchConfiguration> SearchConfigurations => Set<SearchConfiguration>();
    public DbSet<SearchCategory> SearchCategories => Set<SearchCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AStarDevContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=astar-dev.db");
        }
    }
}