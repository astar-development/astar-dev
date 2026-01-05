using System.Diagnostics;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AStar.Dev.FilesDb.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const            string         ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource     = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using IServiceScope scope     = serviceProvider.CreateScope();
            FilesContext        dbContext = scope.ServiceProvider.GetRequiredService<FilesContext>();

            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch(Exception ex)
        {
            _ = (activity?.AddException(ex));
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(FilesContext dbContext, CancellationToken cancellationToken)
    {
        IRelationalDatabaseCreator dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        if(!await dbCreator.ExistsAsync(cancellationToken)) await dbCreator.CreateAsync(cancellationToken);
                                    });
    }

    private static async Task RunMigrationAsync(FilesContext dbContext, CancellationToken cancellationToken)
    {
        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        await dbContext.Database.MigrateAsync(cancellationToken);
                                    });
    }

    private static async Task SeedDataAsync(FilesContext dbContext, CancellationToken cancellationToken)
    {
        var exists = dbContext.Files.Any(detail => detail.DirectoryName.Value == "mock" && detail.FileName.Value == "Fake");

        if(exists) return;

        FileDetail firstTicket = new()
                                 {
                                     DirectoryName    = new DirectoryName("mock"),
                                     FileName         = new FileName("Fake"),
                                     ImageDetail      = new ImageDetail(),
                                     FileHandle       = new FileHandle("Mock Handle"),
                                     FileSize         = 123,
                                     IsImage          = true,
                                     FileAccessDetail = new FileAccessDetail { DetailsLastUpdated = DateTime.UtcNow, },
                                 };

        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                                        _ = await dbContext.Files.AddAsync(firstTicket, cancellationToken);
                                        _ = await dbContext.SaveChangesAsync(cancellationToken);
                                        await transaction.CommitAsync(cancellationToken);
                                    });
    }
}
