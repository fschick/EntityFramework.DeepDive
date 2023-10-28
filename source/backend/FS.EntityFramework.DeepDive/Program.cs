using FS.EntityFramework.DeepDive.Core.Configuration;
using FS.EntityFramework.DeepDive.Repository.Startup;
using FS.EntityFramework.DeepDive.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace FS.EntityFramework.DeepDive;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.RegisterConfiguration();
        builder.Services.RegisterServices(builder.Configuration);
        builder.Services.RegisterRestApi();
        builder.Services.RegisterOpenApi();
        builder.Services.RegisterWebUi(builder.Environment);

        var app = builder.Build();

        app.AddRestApi();
        app.AddOpenApi();

        app.MapRestApi();
        app.MapOpenApi();
        app.MapWebUi();

        await app.MigrateDatabase();

        await app.RunAsync();
    }

    private static void RegisterConfiguration(this WebApplicationBuilder builder)
    {
        var configurationSection = builder.Configuration.GetSection(DeepDiveConfiguration.CONFIGURATION_SECTION);
        builder.Services.Configure<DeepDiveConfiguration>(configurationSection);
    }
}