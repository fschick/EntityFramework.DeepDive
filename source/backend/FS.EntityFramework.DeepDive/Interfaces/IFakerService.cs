using Bogus;
using FS.EntityFramework.DeepDive.Core.Configuration;

namespace FS.EntityFramework.DeepDive.Interfaces;

/// <summary>
/// Service to access database specific faker.
/// </summary>
public interface IFakerService
{
    /// <summary>
    /// Get a faker for the specified database type.
    /// </summary>
    /// <param name="databaseType"></param>
    Faker this[DatabaseType databaseType] { get; }
}