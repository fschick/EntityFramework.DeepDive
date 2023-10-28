#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Bogus;
using FS.EntityFramework.DeepDive.Core.Models.Repository;
using FS.EntityFramework.DeepDive.Extensions;
using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.DbContexts;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Controllers;

/// <summary>
/// Blog service.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
public abstract class BlogController : ControllerBase
{
    private readonly IDeepDiveDbContextFactory _dbContextFactory;
    private readonly IFakerService _fakerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlogController"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    protected BlogController(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
    {
        _dbContextFactory = dbContextFactory;
        _fakerService = fakerService;
    }

    #region #region Operation trace auxiliary methods
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Required for further usage.")]
    protected async Task<ExecutionTrace> Trace(bool codeOnly, Func<DeepDiveDbContext, Faker, Task> source, [CallerArgumentExpression(nameof(source))] string? sourceCode = null)
    {
        return await Trace(codeOnly, sourceCall, sourceCode);

        async Task<object?> sourceCall(DeepDiveDbContext dbContext, Faker faker)
        {
            await source(dbContext, faker);
            return null;
        }
    }

    protected async Task<ExecutionTrace> Trace<TResult>(bool codeOnly, Func<DeepDiveDbContext, Faker, Task<TResult>> source, [CallerArgumentExpression(nameof(source))] string? sourceCode = null)
    {
        var formattedSourcecode = sourceCode.FormatSourceSnippet();

        var executionTrace = new ExecutionTrace { Sourcecode = formattedSourcecode };

        foreach (var databaseType in _dbContextFactory.SupportedDatabaseTypes)
        {
            if (codeOnly)
            {
                var databaseTrace = DatabaseTrace.Empty(databaseType);
                executionTrace.DatabaseTraces.Add(databaseTrace);
            }
            else
            {
                await using var dbContext = _dbContextFactory.Create(databaseType);
                var sqlQueryResult = await source(dbContext, _fakerService[databaseType]);
                var databaseTrace = DatabaseTrace.Create(databaseType, dbContext.ExecutedSqlCommands, sqlQueryResult);
                executionTrace.DatabaseTraces.Add(databaseTrace);
            }
        }

        return executionTrace;
    }
    #endregion Trace methods
}