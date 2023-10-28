using FS.EntityFramework.DeepDive.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace FS.EntityFramework.DeepDive.Startup;

/// <summary>
/// Startup code to register generation of OpenAPI spec and UI.
/// </summary>
internal static class OpenApiStartup
{
    private const string API_NAME = "EF Core Deep Dive API";
    private const string API_VERSION = "v1";

    /// <summary>
    /// Register OpenAPI related services.
    /// </summary>
    /// <param name="services">The services to act on.</param>
    public static void RegisterOpenApi(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.SwaggerDoc(API_VERSION, new OpenApiInfo { Title = API_NAME });

            options.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.AttributeRouteInfo?.Order:00000}");

            var deepDiveXmlDoc = Path.Combine(AppContext.BaseDirectory, "FS.EntityFramework.DeepDive.xml");
            var deepDiveCoreXmlDoc = Path.Combine(AppContext.BaseDirectory, "FS.EntityFramework.DeepDive.Core.xml");
            options.IncludeXmlComments(deepDiveXmlDoc);
            options.IncludeXmlComments(deepDiveCoreXmlDoc);
        });
    }

    /// <summary>
    /// Add OpenAPI spec generation middleware and UI.
    /// </summary>
    /// <param name="app">The app to act on.</param>
    public static void AddOpenApi(this WebApplication app)
    {
        app.AddSwaggerMiddleware();
        app.AddSwaggerUi();
    }

    public static void MapOpenApi(this IEndpointRouteBuilder app)
    {
        app.MapGet($"/{StaticRoutes.OPEN_API_UI_ROUTE}{{**path}}", redirectToOpenApiUi);
        app.MapGet($"/{StaticRoutes.SWAGGER_UI_ROUTE}{{**path}}", redirectToOpenApiUi);

        return;

        static IResult redirectToOpenApiUi(HttpContext httpContext, string? path = null)
        {
            var query = httpContext.Request.QueryString;
            var redirectUrl = $"/{StaticRoutes.API_PREFIX}/{path}{query}";
            return Results.Redirect(redirectUrl, true);
        }
    }

    private static void AddSwaggerMiddleware(this IApplicationBuilder app)
        => app.UseSwagger(c => c.RouteTemplate = $"{StaticRoutes.API_PREFIX}/{{documentName}}/{StaticRoutes.OPEN_API_SPEC}");

    private static void AddSwaggerUi(this IApplicationBuilder app)
    {
        app.UseSwaggerUI(config =>
        {
            config.RoutePrefix = StaticRoutes.API_PREFIX;
            config.SwaggerEndpoint($"{API_VERSION}/{StaticRoutes.OPEN_API_SPEC}", API_NAME);
            config.DisplayRequestDuration();
            config.EnableDeepLinking();
            config.EnableTryItOutByDefault();
            config.ConfigObject.AdditionalItems.Add("requestSnippetsEnabled", true);
        });
    }
}