using FS.EntityFramework.DeepDive.Core.Configuration;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.EntityFramework.DeepDive.Repository.DbContexts;

public class DeepDiveDbContextFactory : IDeepDiveDbContextFactory
{
    private readonly DeepDiveConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Gets the supported database types.
    /// </summary>
    public IReadOnlyCollection<DatabaseType> SupportedDatabaseTypes { get; }

    public DeepDiveDbContextFactory(IOptions<DeepDiveConfiguration> configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration.Value;
        _serviceProvider = serviceProvider;
        SupportedDatabaseTypes = _configuration.Databases.Keys.Reverse().ToList();

        var noDatabaseConfigured = SupportedDatabaseTypes.Count == 0;
        if (noDatabaseConfigured)
            throw new InvalidOperationException("No database configured.");
    }

    public DeepDiveDbContext Create(DatabaseType databaseType)
    {
        if (!_configuration.Databases.TryGetValue(databaseType, out var dbConfig))
            throw new ArgumentOutOfRangeException(nameof(databaseType), "Configured database type is unsupported.");

        if (string.IsNullOrEmpty(dbConfig.ConnectionString))
            throw new ArgumentOutOfRangeException(nameof(databaseType), "Configured connection string is empty.");

        var configuration = _serviceProvider.GetRequiredService<IOptions<DeepDiveConfiguration>>();
        var environment = _serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        var sqlTraceService = _serviceProvider.GetRequiredService<ISqlTraceService>();
        var tableLockInterceptor = _serviceProvider.GetService<TableLockInterceptor>();
        return new DeepDiveDbContext(databaseType, dbConfig.ConnectionString, configuration, environment, loggerFactory, sqlTraceService, tableLockInterceptor);
    }
}