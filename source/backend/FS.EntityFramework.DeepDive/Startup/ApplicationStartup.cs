using FS.EntityFramework.DeepDive.Interfaces;
using FS.EntityFramework.DeepDive.Repository.DbContexts;
using FS.EntityFramework.DeepDive.Repository.Interfaces;
using FS.EntityFramework.DeepDive.Repository.Services;
using FS.EntityFramework.DeepDive.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FS.EntityFramework.DeepDive.Startup;

internal static class ApplicationStartup
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddSingleton<IFakerService, FakerService>();
        services.AddScoped<IDeepDiveDbContextFactory, DeepDiveDbContextFactory>();
        services.AddTransient<ISqlTraceService, SqlTraceService>();
        services.AddSingleton<TableLockInterceptor>();

        // Required for EF Core tools.
        services.AddDbContext<DeepDiveDbContext>();

        return services;
    }
}