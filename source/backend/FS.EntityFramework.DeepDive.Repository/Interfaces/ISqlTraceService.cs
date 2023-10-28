using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FS.EntityFramework.DeepDive.Repository.Interfaces;

public interface ISqlTraceService : IDbCommandInterceptor
{
    IReadOnlyCollection<string> ExecutedSqlCommands { get; }
}