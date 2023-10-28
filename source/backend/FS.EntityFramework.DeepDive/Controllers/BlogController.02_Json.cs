using FS.EntityFramework.DeepDive.Core.Models.Application;
using FS.EntityFramework.DeepDive.Core.Models.Repository;
using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FS.EntityFramework.DeepDive.Controllers;

/// Illustration of simple CRUD operations in EF Core.
[SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
[Tags("JSON Columns"), Route($"{StaticRoutes.API_PREFIX}/Blog/Json/[action]", Order = 2)]
public class BlogControllerJson : BlogController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlogControllerJson"/> class.
    /// </summary>
    /// <param name="dbContextFactory"></param>
    /// <param name="fakerService"></param>
    public BlogControllerJson(IDeepDiveDbContextFactory dbContextFactory, IFakerService fakerService)
        : base(dbContextFactory, fakerService) { }

    /// <summary>
    /// Add and save a new author with address.
    /// </summary>
    /// <param name="codeOnly">Return source code only. Don't execute any statements.</param>
    [HttpPost]
    public async Task<ExecutionTrace> CreateAuthor(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, faker) =>
    {
        var author = new Author
        {
            Name = faker.Name.FullName(),
            Address = new Address
            {
                Street = faker.Address.StreetAddress(),
                PostalCode = faker.Address.ZipCode(),
                City = faker.Address.City(),
            }
        };

        dbContext.Add(author);
        await dbContext.SaveChangesAsync();

        return author;
    });

    /// <summary>
    /// Update the address of an author.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpGet]
    [SuppressMessage("ReSharper", "EntityFramework.NPlusOne.IncompleteDataQuery")]
    [SuppressMessage("ReSharper", "EntityFramework.NPlusOne.IncompleteDataUsage")]
    public async Task<ExecutionTrace> GetAuthorByStreet(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, _) =>
    {
        var streets = new[] { "Max-Pechstein-Str. 7", "Ropenstaller Weg 551" };
        return await dbContext.Authors.Where(author => streets.Contains(author.Address!.Street)).ToListAsync();
    });

    /// <summary>
    /// Update the address of an author.
    /// </summary>
    /// <param name="codeOnly"></param>
    [HttpPut]
    [SuppressMessage("ReSharper", "EntityFramework.NPlusOne.IncompleteDataQuery")]
    [SuppressMessage("ReSharper", "EntityFramework.NPlusOne.IncompleteDataUsage")]
    public async Task<ExecutionTrace> UpdateAuthorsAddress(bool codeOnly = false) => await Trace(codeOnly, async (dbContext, faker) =>
    {
        var author = dbContext.Authors.FirstOrDefault(author => author.Address != null);
        if (author == null)
            return null;

        author.Address!.Street = faker.Address.StreetAddress();

        await dbContext.SaveChangesAsync();

        return author;
    });
}