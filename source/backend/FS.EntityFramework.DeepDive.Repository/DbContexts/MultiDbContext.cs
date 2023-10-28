using FS.EntityFramework.DeepDive.Core.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FS.EntityFramework.DeepDive.Repository.DbContexts;

public abstract class MultiDbContext : DbContext
{
    private readonly string _connectionString;
    public readonly DatabaseType DatabaseType;

    protected MultiDbContext(DatabaseType databaseType, string connectionString)
    {
        DatabaseType = databaseType;
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        switch (DatabaseType)
        {
            case DatabaseType.InMemory:
                ConfigureInMemoryDatabase(optionsBuilder);
                break;
            case DatabaseType.SqlServer:
                ConfigureSqlServerDatabase(optionsBuilder);
                break;
            case DatabaseType.Sqlite:
                ConfigureSqliteDatabase(optionsBuilder);
                break;
            case DatabaseType.MySql:
                ConfigureMySqlDatabase(optionsBuilder);
                break;
            default:
                throw new ArgumentOutOfRangeException(null, "Configured database type is unsupported");
        }
    }

    #region Database configuration
    private void ConfigureInMemoryDatabase(DbContextOptionsBuilder optionsBuilder)
    {
        var migrationAssembly = GetMigrationAssembly();
        optionsBuilder.UseSqlite(_connectionString, o => o.MigrationsAssembly(migrationAssembly));
    }

    private void ConfigureSqliteDatabase(DbContextOptionsBuilder optionsBuilder)
    {
        var migrationAssembly = GetMigrationAssembly();
        var connectionStringBuilder = new SqliteConnectionStringBuilder(_connectionString);
        var databaseDirectory = Path.GetDirectoryName(connectionStringBuilder.DataSource);
        CreateDatabaseDirectory(databaseDirectory);
        optionsBuilder.UseSqlite(_connectionString, o => o.MigrationsAssembly(migrationAssembly));
    }

    private void ConfigureSqlServerDatabase(DbContextOptionsBuilder optionsBuilder)
    {
        var migrationAssembly = GetMigrationAssembly();
        optionsBuilder.UseSqlServer(_connectionString, o => o.MigrationsAssembly(migrationAssembly));
    }

    private void ConfigureMySqlDatabase(DbContextOptionsBuilder optionsBuilder)
    {
        var migrationAssembly = GetMigrationAssembly();
        var serverVersion = ServerVersion.AutoDetect(_connectionString);
        optionsBuilder.UseMySql(_connectionString, serverVersion, o => o.MigrationsAssembly(migrationAssembly));
    }
    #endregion

    #region EF Core CLI stuff
    protected static DatabaseType GetDatabaseType()
    {
        var commandLineArgs = Environment.GetCommandLineArgs().ToList();
        var migrationArgIndex = commandLineArgs.IndexOf("migrations");
        var assemblyArgumentIndex = commandLineArgs.IndexOf("--assembly");

        var isCalledFromEfTool = migrationArgIndex == 1 && assemblyArgumentIndex > 0;
        if (!isCalledFromEfTool)
            throw new InvalidOperationException("This constructor is for EF Core CLI tools only");

        var assembly = commandLineArgs[assemblyArgumentIndex + 1];
        var databaseTypeString = Regex.Match(assembly, @"Repository\.(?<databasetype>.*)\.dll$").Groups["databasetype"].Value;

        if (!Enum.TryParse<DatabaseType>(databaseTypeString, out var databaseType))
            throw new InvalidOperationException("Your target project doesn't match your migrations assembly. Either change your target project or change your migrations assembly.");

        return databaseType;
    }

    protected static string GetConnectionString(IOptions<DeepDiveConfiguration> configuration)
    {
        var databaseType = GetDatabaseType();

        if (!configuration.Value.Databases.TryGetValue(databaseType, out var dbConfig))
            throw new InvalidOperationException($"No connection string found for database type {databaseType}");

        return dbConfig.ConnectionString;
    }
    #endregion

    private string GetMigrationAssembly()
    {
        var repositoryAssemblyName = typeof(MultiDbContext).Assembly.GetName().Name;
        return DatabaseType switch
        {
            DatabaseType.InMemory => $"{repositoryAssemblyName}.{DatabaseType.Sqlite}",
            _ => $"{repositoryAssemblyName}.{DatabaseType}",
        };
    }

    private static void CreateDatabaseDirectory(string? databaseDirectory)
    {
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
            Directory.CreateDirectory(databaseDirectory);
    }
}