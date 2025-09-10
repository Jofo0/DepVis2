using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DepVis.ServiceDefaults;

public static class ServiceDefaults
{
    /// <summary>
    /// Configures the defaults for the platform
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="massTransitConfig"></param>
    /// <returns></returns>
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.Services.ConfigureMassTransit(builder.Configuration);

        return builder;
    }

    private static IServiceCollection ConfigureMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? massTransitConfig = null,
        bool includeAll = true
    )
    {
        Log.Information("Configuring MassTransit");

        var sqlConnectionString = configuration.GetConnectionString("DefaultConnection");

        services
            .AddOptions<SqlTransportOptions>()
            .Configure(options =>
            {
                options.ConnectionString = sqlConnectionString;
            });

        services.AddSqlServerMigrationHostedService(x =>
        {
            x.CreateDatabase = false;
        });

        services.AddMassTransit(x =>
        {
            x.AddSqlMessageScheduler();
            x.DisableUsageTelemetry();
            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var entryAssembly = Assembly.GetEntryAssembly();

            x.AddConsumers(entryAssembly);

            massTransitConfig?.Invoke(x);

            x.UsingSqlServer(
                (ctx, cfg) =>
                {
                    cfg.UseSqlMessageScheduler();
                    cfg.ConfigureEndpoints(ctx);
                }
            );
        });

        return services;
    }
}
