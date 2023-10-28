using FS.EntityFramework.DeepDive.Repository.DbContexts;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Repository.Startup;

public static class DatabaseMigrationStartup
{
    public static async Task MigrateDatabase(this WebApplication webApplication, CancellationToken cancellationToken = default)
    {
        var serviceFactory = webApplication.Services.GetRequiredService<IServiceScopeFactory>();
        using var serviceScope = serviceFactory.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        await MigrateDatabase(serviceProvider, cancellationToken);
    }

    private static async Task MigrateDatabase(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<DeepDiveDbContext>>();
        var dbContextFactory = serviceProvider.GetRequiredService<IDeepDiveDbContextFactory>();

        foreach (var databaseType in dbContextFactory.SupportedDatabaseTypes)
        {
            await using var dbContext = dbContextFactory.Create(databaseType);
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pendingMigrations.Count == 0)
                continue;

            logger.LogInformation($"Apply migrations to {databaseType} database. Please be patient ...");
            foreach (var pendingMigration in pendingMigrations)
                logger.LogInformation(pendingMigration);

            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Database migration finished.");
        }
    }
}