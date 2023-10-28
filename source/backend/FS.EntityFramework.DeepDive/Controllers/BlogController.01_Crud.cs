using Bogus;
using FS.EntityFramework.DeepDive.Core.Models.Application;
using FS.EntityFramework.DeepDive.Core.Models.Repository;
using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Routing;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Controllers;

/// Illustration of simple CRUD operations in EF Core.
[SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
[Tags("CRUD"), Route($"{StaticRoutes.API_PREFIX}/Blog/Crud/[action]", Order = 1)]
public class BlogControllerCrud : BlogController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlogControllerCrud"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    public BlogControllerCrud(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
        : base(dbContextFactory, fakerService) { }

    /// <summary>
    /// Add and save a new blog.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpPost]
    public async Task<ExecutionTrace> CreateBlog(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, faker) =>
    {
        var blog = new Blog
        {
            Title = GetBlogTitle(faker),
            Author = new Author { Name = faker.Name.FullName() },
            Tags = faker.Commerce.Categories(2).Select(category => new Tag { Name = category }).ToList()
        };

        dbContext.Add(blog);
        await dbContext.SaveChangesAsync();

        return blog;
    });

    /// <summary>
    /// Add and save two blogs with referentially identical tags.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpPost]
    public async Task<ExecutionTrace> CreateTwoBlogWithSameTags(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, faker) =>
    {
        var blog1 = new Blog
        {
            Title = GetBlogTitle(faker),
            Author = new Author { Name = faker.Name.FullName() },
        };

        var blog2 = new Blog
        {
            Title = GetBlogTitle(faker),
            Author = new Author { Name = faker.Name.FullName() },
        };

        var tags = faker.Commerce.Categories(2).Select(category => new Tag { Name = category }).ToList();
        blog1.Tags = tags;
        blog2.Tags = tags;

        dbContext.AddRange(blog1, blog2);
        await dbContext.SaveChangesAsync();

        return new[] { blog1, blog2 };
    });

    /// <summary>
    /// Select all blogs without navigation properties.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogs(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        return await dbContext.Blogs.ToListAsync();
    });

    /// <summary>
    /// Select first blog from 'Corinna Franzis' with navigation properties.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlog(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        return await dbContext.Blogs
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .Where(blog => blog.Author!.Name == "Corinna Franzis")
            .FirstOrDefaultAsync();
    });

    /// <summary>
    /// Select first blog from 'Corinna Franzis' with tags from separate query.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpGet]
    public async Task<ExecutionTrace> GetBlogManual(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var blog = await dbContext.Blogs
            .Include(x => x.Author)
            .Where(blog => blog.Author!.Name == "Corinna Franzis")
            .FirstOrDefaultAsync();

        if (blog == null)
            return null;

        var tags = from tag in dbContext.Tags
                   from blogTag in dbContext.Set<BlogTag>()
                   where blogTag.BlogId == blog.Id && blogTag.TagId == tag.Id
                   select tag;

        blog.Tags = await tags.ToListAsync();

        return blog;
    });

    /// <summary>
    /// Update the title of the first blog found.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpPut]
    public async Task<ExecutionTrace> UpdateBlogTitle(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var blog = await dbContext.Blogs
            .FirstOrDefaultAsync();

        if (blog == null)
            return null;

        blog.Title = $"{blog.Title} (modified)";
        await dbContext.SaveChangesAsync();

        return blog;
    });

    /// <summary>
    /// Set the title of the first blog found and update it with an explicit update.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpPut]
    public async Task<ExecutionTrace> UpdateBlogExplicit(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var blog = await dbContext.Blogs
            .FirstOrDefaultAsync();

        if (blog == null)
            return null;

        blog.Title = $"{blog.Title} (modified)";
        dbContext.Update(blog);
        await dbContext.SaveChangesAsync();

        return blog;
    });

    /// <summary>
    /// Update the title of all blogs to its origin value using a bulk update.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpPut]
    public async Task<ExecutionTrace> ResetBlogTitles(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        // ExecuteUpdate saves data without using change tracking and SaveChanges().
        var rowUpdateCount = await dbContext.Blogs
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(x => x.Title, x => x.Title.Replace(" (modified)", ""))
            );

        return rowUpdateCount;
    });

    /// <summary>
    /// Delete blogs from 'Corinna Franzis'.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpDelete]
    public async Task<ExecutionTrace> DeleteBlogs(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        // ExecuteDelete modifies data without using change tracking and SaveChanges().
        var rowDeletionCount = await dbContext.Blogs
            .Where(blog => blog.Author!.Name == "Corinna Franzis")
            .ExecuteDeleteAsync();

        return rowDeletionCount;
    });

    /// <summary>
    /// Delete all data using a transaction.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpDelete]
    public async Task<ExecutionTrace> DeleteAllData(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        // We need to use am explicit transaction to ensure data consistency 
        // because ExecuteDeleteAsync does not use change tracking.
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        await dbContext.Blogs
            .ExecuteDeleteAsync();

        await dbContext.Authors
            .ExecuteDeleteAsync();

        await dbContext.Tags
            .ExecuteDeleteAsync();

        await transaction.CommitAsync();
    });

    private static string GetBlogTitle(Faker faker)
        => $"{faker.Hacker.Verb()} the {faker.Hacker.Noun()}".Humanize(LetterCasing.Sentence);
}