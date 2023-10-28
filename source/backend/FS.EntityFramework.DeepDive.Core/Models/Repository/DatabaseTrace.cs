using FS.EntityFramework.DeepDive.Core.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FS.EntityFramework.DeepDive.Core.Models.Repository;

/// <summary>
/// Database specific execution trace.
/// </summary>
public class DatabaseTrace
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };

    /// <summary>
    /// The type of the database.
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// SQL commands executed by the database.
    /// </summary>
    public required List<string> SqlCommands { get; set; }

    /// <summary>
    /// Result returned by the database.
    /// </summary>
    public required object Result { get; set; }

    /// <summary>
    /// Result returned by the database as JSON.
    /// </summary>
    public string ResultJson => JsonConvert.SerializeObject(Result, _jsonSerializerSettings);

    /// <summary>
    /// Factory method to create a database trace.
    /// </summary>
    /// <param name="databaseType">The type of the database.</param>
    /// <param name="sqlCommands">The executed SQL statements.</param>
    /// <param name="result">The result from database</param>
    public static DatabaseTrace Create<TResult>(DatabaseType databaseType, IEnumerable<string> sqlCommands, TResult? result)
        => new()
        {
            DatabaseType = databaseType,
            SqlCommands = sqlCommands.ToList(),
            Result = result ?? new object()
        };

    /// <summary>
    /// Factory method to create an empty database trace.
    /// </summary>
    /// <param name="databaseType">The type of the database.</param>
    public static DatabaseTrace Empty(DatabaseType databaseType)
        => new()
        {
            DatabaseType = databaseType,
            SqlCommands = [],
            Result = new object(),
        };
}