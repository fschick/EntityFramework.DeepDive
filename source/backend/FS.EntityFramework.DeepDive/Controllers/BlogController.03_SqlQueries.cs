using FS.EntityFramework.DeepDive.Core.Models.Application;
using FS.EntityFramework.DeepDive.Core.Models.Repository;
using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Controllers;

/// <summary>
/// Illustration using direct SQL in EF Core.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
[Tags("SQL Queries"), Route($"{StaticRoutes.API_PREFIX}/Blog/SqlQueries/[action]", Order = 3)]
public class BlogControllerDirectSql : BlogController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlogControllerDirectSql"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    public BlogControllerDirectSql(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
        : base(dbContextFactory, fakerService) { }

    /// <summary>
    /// Select blogs having status 'Published' using a stored procedure.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    public async Task<ExecutionTrace> GetPublishedBlogs(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        if (dbContext.Database.IsSqlServer())
            return await dbContext.Blogs
                .FromSql($"EXECUTE GetPublishedBlogs")
                .ToListAsync();

        if (dbContext.Database.IsMySql())
            return await dbContext.Blogs
                .FromSql($"CALL GetPublishedBlogs")
                .ToListAsync();

        return await dbContext.Blogs
            .FromSql($"SELECT * FROM Blogs WHERE Status = 'Published'")
            .ToListAsync();
    });

    /// <summary>
    /// Select columns of blog table having default values using direct SQL commands.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogColumnsHavingDefaults(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var blogTableName = dbContext.Model
            .FindEntityType(typeof(Blog))!
            .GetTableName();

        IQueryable<ColumnInformation> query;
        if (dbContext.Database.IsSqlServer() || dbContext.Database.IsMySql())
        {
            query = dbContext.Database
                .SqlQuery<ColumnInformation>($"""
                    SELECT 
                        TABLE_NAME AS TableName,
                        COLUMN_NAME AS ColumnName,
                        ORDINAL_POSITION AS Position,
                        COLUMN_DEFAULT AS DefaultValue
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = {blogTableName}
                    """);
        }
        else if (dbContext.Database.IsSqlite())
        {
            query = dbContext.Database
                .SqlQuery<ColumnInformation>($"""
                    SELECT
                        {blogTableName} AS TableName,
                        name AS ColumnName,
                        cid AS Position,
                        dflt_value AS DefaultValue
                    FROM pragma_table_info({blogTableName})
                    """);
        }
        else
        {
            throw new NotSupportedException($"Database type {dbContext.Database.ProviderName} is not supported");
        }

        return await query
            .Where(column => column.DefaultValue != null)
            .ToListAsync();
    });

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class ColumnInformation
    {
        public required string TableName { get; set; }
        public required string ColumnName { get; set; }
        public required int Position { get; set; }
        public string? DefaultValue { get; set; }
    }
}