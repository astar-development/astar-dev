using System.Diagnostics;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AStar.Dev.FilesDb.MigrationService;

public class DatabaseMigrationService(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, ILogger<DatabaseMigrationService> logger)
    : BackgroundService
{
    private const           string         ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource     = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting database migration service");
        // ReSharper disable once ExplicitCallerInfoArgument
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope     = serviceProvider.CreateScope();
            var       dbContext = scope.ServiceProvider.GetRequiredService<FilesContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch(Exception ex)
        {
            activity?.AddException(ex);
            logger.LogError(ex, "Error migrating database");
        }

        logger.LogInformation("Stopping database migration service");
        hostApplicationLifetime.StopApplication();
    }

    private async Task EnsureDatabaseAsync(FilesContext dbContext, CancellationToken stoppingToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        logger.LogInformation("Ensuring database exists");
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
                                    {
                                        if(!await dbCreator.ExistsAsync(stoppingToken))
                                        {
                                            await dbCreator.CreateAsync(stoppingToken);
                                        }
                                    });
    }

    private async Task RunMigrationAsync(FilesContext dbContext, CancellationToken stoppingToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        logger.LogInformation("Running migrations");
        await strategy.ExecuteAsync(async () =>
                                    {
                                        await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);
                                        await dbContext.Database.MigrateAsync(stoppingToken);
                                        await transaction.CommitAsync(stoppingToken);
                                    });
    }

    private async Task SeedDataAsync(FilesContext dbContext, CancellationToken stoppingToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        logger.LogInformation("Seeding data");
        await strategy.ExecuteAsync(async () =>
                                    {
                                        if(!await dbContext.Files.AnyAsync(stoppingToken))
                                        {
                                            var fileDetail = new FileDetail
                                                             {
                                                                 FileName      = new("MockFileName.jpg"),
                                                                 DirectoryName = new("MockDirectoryName"),
                                                                 CreatedDate   = DateTimeOffset.UtcNow,
                                                                 FileHandle    = new("MockFileName-jpg"),
                                                                 FileSize      = 12345,
                                                                 IsImage       = true,
                                                                 ImageDetail   = new(1234, 5678)
                                                             };

                                            var exists = await dbContext.Files.AnyAsync(x => x.FileHandle == fileDetail.FileHandle, stoppingToken);

                                            if(exists)
                                            {
                                                logger.LogInformation("Mock File already exists in database, so exiting");

                                                return;
                                            }

                                            await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                                            await dbContext.Files.AddAsync(fileDetail, stoppingToken);
                                            await dbContext.SaveChangesAsync(stoppingToken);
                                            await transaction.CommitAsync(stoppingToken);
                                        }
                                    });
    }
}
