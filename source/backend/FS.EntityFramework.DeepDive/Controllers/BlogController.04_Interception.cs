using FS.EntityFramework.DeepDive.Core.Models.Application;
using FS.EntityFramework.DeepDive.Core.Models.Repository;
using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Services;
using FS.EntityFramework.DeepDive.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Controllers;

/// <summary>
/// Illustration of interception in EF Core.
/// </summary>
[SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
[Tags("Interception"), Route($"{StaticRoutes.API_PREFIX}/Blog/Interception/[action]", Order = 4)]
public class BlogControllerInterception : BlogController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlogControllerInterception"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    public BlogControllerInterception(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
        : base(dbContextFactory, fakerService) { }

    /// <summary>
    ///Set the status of a blog to published.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpPost]
    public async Task<ExecutionTrace> UpdateBlogStatusToPublished(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var blog = await dbContext.Blogs
            .Where(b => b.Status == BlogStatus.Draft)
            .FirstOrDefaultAsync();

        if (blog == null)
            return null;

        blog.Status = BlogStatus.Published;
        await dbContext.SaveChangesAsync();

        return blog;
    });

    /// <summary>
    /// Select blogs with table lock (SQL Server only).
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogsWithTableLock(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        return await dbContext.Blogs
            .TagWith(TableLockInterceptor.USE_TABLE_LOCK)
            .ToListAsync();
    });
}