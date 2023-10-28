using FS.EntityFramework.DeepDive.Core.Models.Repository;
using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Extensions;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Controllers;

/// <summary>
/// Illustration using direct SQL in EF Core.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
[Tags("SQL Expressions"), Route($"{StaticRoutes.API_PREFIX}/Blog/SqlExpressions/[action]", Order = 6)]
public class BlogControllerSqlExpressions : BlogController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlogControllerSqlExpressions"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    public BlogControllerSqlExpressions(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
        : base(dbContextFactory, fakerService) { }

    /// <summary>
    /// Select blogs by partial unique identifier.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogByPartialId(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var blogId = dbContext.Blogs.Select(blog => blog.Id).FirstOrDefault();
        var partialBlogIdPattern = $"{blogId.ToString()[..8]}%";

        return await dbContext.Blogs
            .Where(blog => blog.Id.Like(partialBlogIdPattern))
            .ToListAsync();
    });
}