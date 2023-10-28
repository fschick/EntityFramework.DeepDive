using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

namespace FS.EntityFramework.DeepDive.Startup;

internal static class WebUiStartup
{
    public static void RegisterWebUi(this IServiceCollection services, IHostEnvironment hostEnvironment)
        => services.AddSpaStaticFiles(configuration => configuration.RootPath = GetWebRootPath(hostEnvironment));

    public static void MapWebUi(this WebApplication app)
    {
        app.UseSpaStaticFiles();
        app.UseSpa(_ => { });
    }

    private static string GetWebRootPath(IHostEnvironment hostEnvironment)
        => hostEnvironment.IsProduction()
            ? Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "WebUi")
            : Path.Combine("..", "..", "frontend", "dist", "ef-deep-dive", "browser");
}