using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AStarDev.OneDriveSyncClient.Persistence;

public class AStarDevContextFactory : IDesignTimeDbContextFactory<AStarDevContext>
{
    public AStarDevContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AStarDevContext>();
        optionsBuilder.UseSqlite("Data Source=astar-dev.db");

        return new AStarDevContext(optionsBuilder.Options);
    }
}
