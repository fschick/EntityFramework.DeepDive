namespace FS.EntityFramework.DeepDive.Core.Configuration;

/// <summary>
/// Database types supported by this application
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// Sqlite in memory (unit testing only)
    /// </summary>
    InMemory,

    /// <summary>
    /// Microsoft Sql Server
    /// </summary>
    SqlServer,

    /// <summary>
    /// SQLite
    /// </summary>
    Sqlite,

    /// <summary>
    /// My SQL / Maria DB
    /// </summary>
    MySql,
}