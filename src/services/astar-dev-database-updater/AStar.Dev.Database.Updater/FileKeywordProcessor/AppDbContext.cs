using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Database.Updater.FileKeywordProcessor;

public class AppDbContext : DbContext
{
    public DbSet<FileKeywordMatch> FileKeywordMatches { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=filesDb2;User Id=sa;Password=<SecurePasswordHere1!>;TrustServerCertificate=true;");
}
