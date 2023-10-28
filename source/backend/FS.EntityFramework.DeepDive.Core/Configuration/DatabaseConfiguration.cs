namespace FS.EntityFramework.DeepDive.Core.Configuration;

/// <summary>
/// Database specific configuration.
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// The connection string for the database.
    /// </summary>
    public required string ConnectionString { get; set; }
}