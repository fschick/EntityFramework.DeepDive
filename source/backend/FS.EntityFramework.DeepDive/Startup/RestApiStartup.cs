using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace FS.EntityFramework.DeepDive.Startup;

/// <summary>
/// Startup code to register REST API services and configuration (CORS, ...).
/// </summary>
internal static class RestApiStartup
{
    /// <summary>
    /// Register REST API related services.
    /// </summary>
    /// <param name="services">The services to act on.</param>
    public static void RegisterRestApi(this IServiceCollection services)
        => services
            .AddControllers()
            .AddNewtonsoftJson(options =>
            {
                var camelCase = new CamelCaseNamingStrategy();
                options.SerializerSettings.Converters.Add(new StringEnumConverter(camelCase));
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

    /// <summary>
    /// Configure CORS.
    /// </summary>
    /// <param name="app">The app to act on.</param>
    public static void AddRestApi(this WebApplication app)
        => app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    /// <summary>
    /// Register REST routes.
    /// </summary>
    /// <param name="app">The app to act on.</param>
    public static void MapRestApi(this WebApplication app)
        => app.MapControllers();
}