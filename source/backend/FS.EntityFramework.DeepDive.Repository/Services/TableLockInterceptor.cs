using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Repository.Services;

public class TableLockInterceptor : DbCommandInterceptor
{
    public const string USE_TABLE_LOCK = "Use table lock";

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        AddTableLockIfRequested(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        AddTableLockIfRequested(command);
        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    private static void AddTableLockIfRequested(IDbCommand command)
    {
        if (command.CommandText.StartsWith($"-- {USE_TABLE_LOCK}"))
            command.CommandText += " WITH (TABLOCKX, HOLDLOCK)";
    }
}