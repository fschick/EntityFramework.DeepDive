using FS.EntityFramework.DeepDive.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Repository.Services;

public class SqlTraceService : ISqlTraceService
{
    private readonly List<string> _executedCommands = [];

    public IReadOnlyCollection<string> ExecutedSqlCommands => _executedCommands;

    public DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        _executedCommands.Add(command.GetFullCommandText());
        return result;
    }

    public object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        _executedCommands.Add(command.GetFullCommandText());
        return result;
    }

    public int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        _executedCommands.Add(command.GetFullCommandText());
        return result;
    }

    public ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        _executedCommands.Add(command.GetFullCommandText());
        return new ValueTask<DbDataReader>(result);
    }

    public ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        _executedCommands.Add(command.GetFullCommandText());
        return new ValueTask<object?>(result);
    }

    public ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        _executedCommands.Add(command.GetFullCommandText());
        return new ValueTask<int>(result);
    }
}

file static class CommandTextExtensions
{
    public static string GetFullCommandText(this DbCommand command)
    {
        var commandText = command.CommandText;
        foreach (DbParameter parameter in command.Parameters)
        {
            var parameterName = $"@{parameter.ParameterName.TrimStart('@')}";
            switch (parameter.Value)
            {
                case null:
                case DBNull:
                    commandText = commandText.Replace(parameterName, "NULL");
                    break;
                case string stringValue:
                    stringValue = stringValue.Replace("'", "''");
                    commandText = commandText.ReplaceParameter(parameterName, $"'{stringValue}'");
                    break;
                case DateTime dateTimeValue:
                    commandText = commandText.ReplaceParameter(parameterName, $"'{dateTimeValue:yyyy-MM-dd HH:mm:ss.fff}'");
                    break;
                case DateTimeOffset dateTimeOffsetValue:
                    commandText = commandText.ReplaceParameter(parameterName, $"'{dateTimeOffsetValue:o}'");
                    break;
                case Guid guidValue:
                    commandText = commandText.ReplaceParameter(parameterName, $"'{guidValue}'");
                    break;
                default:
                    commandText = commandText.ReplaceParameter(parameterName, parameter.Value.ToString());
                    break;
            }
        }

        return commandText;
    }

    private static string ReplaceParameter(this string commandText, string parameterName, string? replacement)
        => Regex.Replace(commandText, $@"{parameterName}\b", replacement ?? string.Empty);
}