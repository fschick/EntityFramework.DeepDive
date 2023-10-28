using FS.EntityFramework.DeepDive.Core.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Repository.Extensions;

public static class StringExtensions
{
    private static readonly MethodInfo _obfuscate = typeof(StringExtensions).GetMethod(nameof(Obfuscate), new[] { typeof(string) })!;

    public static string Obfuscate(this string input)
        => Regex.Replace(input, @"(?<!(^|\s))\w", "*");

    /// <summary>
    /// Registers <see cref="Obfuscate(string)"/> to the entity framework.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="databaseType">Type of the database.</param>
    public static void RegisterObfuscateFunction(this ModelBuilder modelBuilder, DatabaseType databaseType)
    {
        if (databaseType == DatabaseType.Sqlite)
            throw new NotSupportedException("SQLite does not support user defined functions");

        modelBuilder
           .HasDbFunction(_obfuscate)
           .HasName(nameof(Obfuscate))
           .IsNullable();
    }

    /// <summary>
    /// Registers <see cref="Obfuscate(string)"/> to the entity framework.
    /// </summary>
    /// <param name="optionsBuilder">The option builder.</param>
    /// <param name="databaseType">Type of the database.</param>
    public static void RegisterObfuscateFunction(this DbContextOptionsBuilder optionsBuilder, DatabaseType databaseType)
    {
        if (databaseType != DatabaseType.Sqlite)
            throw new NotSupportedException("Only SQLite supports in-memory functions");

        optionsBuilder.AddInterceptors(new ObfuscateFunctionsInterceptor());
    }

    private class ObfuscateFunctionsInterceptor : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            base.ConnectionOpened(connection, eventData);
            CreateFunctionObfuscate((SqliteConnection)connection);
        }

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
            CreateFunctionObfuscate((SqliteConnection)connection);
        }

        private static void CreateFunctionObfuscate(SqliteConnection connection)
            => connection.CreateFunction(nameof(Obfuscate), (Func<string, string>)Obfuscate, isDeterministic: true);
    }
}