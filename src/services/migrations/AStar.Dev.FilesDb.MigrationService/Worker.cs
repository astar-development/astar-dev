using System.Diagnostics;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AStar.Dev.FilesDb.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
    : BackgroundService
{
    private const           string         ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource     = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope     = serviceProvider.CreateScope();
            var       dbContext = scope.ServiceProvider.GetRequiredService<FilesContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);

            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(FilesContext dbContext, CancellationToken stoppingToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        if (!await dbCreator.ExistsAsync(stoppingToken))
                                        {
                                            await dbCreator.CreateAsync(stoppingToken);
                                        }
                                    });
    }

    private static async Task RunMigrationAsync(FilesContext dbContext, CancellationToken stoppingToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);
                                        await dbContext.Database.MigrateAsync(stoppingToken);
                                        await transaction.CommitAsync(stoppingToken);
                                    });
    }

    private static async Task SeedDataAsync(FilesContext dbContext, CancellationToken stoppingToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        if (!await dbContext.FileDetails.AnyAsync(stoppingToken))
                                        {
                                            var fileDetail = new FileDetail
                                                             {
                                                                 FileName      = "MockFileName.jpg",
                                                                 DirectoryName = "MockDirectoryName",
                                                                 FileCreated   = DateTimeOffset.UtcNow,
                                                                 FileHandle    = "MockFileHandle",
                                                                 FileSize      = 12345,
                                                                 IsImage       = true,
                                                                 ImageDetail   = new(1234, 5678)
                                                             };

                                            var             @event      = new Event
                                                                          {
                                                                              DirectoryName    = "MockDirectoryName",
                                                                              FileName         = "MockFileName.jpg",
                                                                              FileSize         = 12345,
                                                                              FileCreated      = DateTimeOffset.UtcNow,
                                                                              Height           = 5678,
                                                                              Width            = 1234,
                                                                              EventOccurredAt  = DateTimeOffset.UtcNow,
                                                                              Type             = EventType.Add,
                                                                              FileLastModified = DateTimeOffset.UtcNow,
                                                                              Handle           = "MockFileHandle",
                                                                              UpdatedBy        = "Jason Barden"
                                                                          };

                                            await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                                            await dbContext.Events.AddAsync(@event, stoppingToken);
                                            await dbContext.FileDetails.AddAsync(fileDetail, stoppingToken);
                                            await dbContext.SaveChangesAsync(stoppingToken);
                                            await transaction.CommitAsync(stoppingToken);
                                        }
                                    });
    }
}
