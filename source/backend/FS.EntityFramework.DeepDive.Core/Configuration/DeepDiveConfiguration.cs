using System.Collections.Generic;

namespace FS.EntityFramework.DeepDive.Core.Configuration;

/// <summary>
/// Application configuration.
/// </summary>
public class DeepDiveConfiguration
{
    /// <summary>
    /// The name of the configuration section for this application.
    /// </summary>
    public const string CONFIGURATION_SECTION = "DeepDive";

    /// <summary>
    /// Database connection strings consists of a key value pair of <see cref="DatabaseType"/> and connection string.
    /// </summary>
    public required Dictionary<DatabaseType, DatabaseConfiguration> Databases { get; set; } = new();
}