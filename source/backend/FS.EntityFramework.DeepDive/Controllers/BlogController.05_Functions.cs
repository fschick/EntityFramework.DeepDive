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
[Tags("Custom functions"), Route($"{StaticRoutes.API_PREFIX}/Blog/Functions/[action]", Order = 5)]
public class BlogControllerFunctions : BlogController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlogControllerFunctions"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    public BlogControllerFunctions(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
        : base(dbContextFactory, fakerService) { }

    /// <summary>
    /// Select blogs where title is LIKE '%Transmitter%'.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogsWhereTitleContainsTransmitter(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        return await dbContext.Blogs
            .Select(blog => new { blog.Id, blog.Title })
            .Where(blog => EF.Functions.Like(blog.Title, "%Transmitter%"))
            .ToArrayAsync();
    });

    /// <summary>
    /// Select blogs with title using DB function 'Obfuscate'.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogsWithObfuscatedTitles(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        return await dbContext.Blogs
            .Select(blog => new { blog.Id, ObfuscatedTitle = blog.Title.Obfuscate(), OriginTitle = blog.Title })
            .ToArrayAsync();
    });
}