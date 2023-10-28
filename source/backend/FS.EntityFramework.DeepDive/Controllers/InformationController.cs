using FS.EntityFramework.DeepDive.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive.Controllers;

/// <summary>
/// Product information API.
/// </summary>
[Route($"{StaticRoutes.API_PREFIX}/[controller]/[action]", Order = 100)]
public class InformationController : ControllerBase
{
    /// <summary>
    /// Gets the product name
    /// </summary>
    /// <param name="cancellationToken"> a token that allows processing to be cancelled</param>
    /// <returns>
    /// The product name
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    public Task<string?> GetProductName(CancellationToken cancellationToken = default)
        => Task.FromResult(Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product);

    /// <summary>
    /// Gets the product version
    /// </summary>
    /// <param name="cancellationToken">a token that allows processing to be cancelled</param>
    /// <returns>
    /// The product version
    /// </returns>
    [HttpGet]
    [AllowAnonymous]
    public Task<string?> GetProductVersion(CancellationToken cancellationToken = default)
        => Task.FromResult(Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
}