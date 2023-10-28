using FS.EntityFramework.DeepDive.Core.Configuration;
using FS.EntityFramework.DeepDive.Repository.DbContexts;
using System.Collections.Generic;

namespace FS.EntityFramework.DeepDive.Repository.Interfaces;

public interface IDeepDiveDbContextFactory
{
    IReadOnlyCollection<DatabaseType> SupportedDatabaseTypes { get; }
    DeepDiveDbContext Create(DatabaseType databaseType);
}