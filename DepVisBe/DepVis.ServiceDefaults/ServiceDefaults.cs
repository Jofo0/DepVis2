using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;

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
        builder.Services.ConfigureLogging(builder.Configuration);
        builder.Services.ConfigureMassTransit(builder.Configuration);

        return builder;
    }

    private static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration, Action<IBusRegistrationConfigurator>? massTransitConfig = null, bool includeAll = true)
    {
        var messageBusProvider = configuration.GetValue<MessageBusProvider>("ServiceProviders:MessageBusProvider");

        Log.Information("Configuring MassTransit with provider: {Provider}", messageBusProvider);

        if (messageBusProvider == MessageBusProvider.SqlDb)
        {
            services.AddSqlServerMigrationHostedService(options =>
            {
                options.CreateInfrastructure = true;
                options.CreateDatabase = false;
                options.DeleteDatabase = false;
            });

            var sqlOptions = configuration.GetRequiredSection("SqlDatabase").Get<SqlDatabaseOptions>();
            if (sqlOptions is null)
            {
                throw new MissingConfigurationException("SqlDatabase");
            }

            if (sqlOptions.UseManagedIdentity)
            {
                throw new NotSupportedException("Managed identity currently not supported");
            }

            services.AddOptions<SqlTransportOptions>().Configure(options =>
            {
                options.Host = sqlOptions.Server;
                options.Database = sqlOptions.PrimaryDatabaseName;
                options.Schema = "transport";
                options.Role = "transport";
                options.Username = sqlOptions.Username;
                options.Password = sqlOptions.Password;
            });
        }

        services.AddMassTransit(x =>
        {
            x.DisableUsageTelemetry();

            x.SetKebabCaseEndpointNameFormatter();
            x.SetInMemorySagaRepositoryProvider();

            var entryAssembly = Assembly.GetEntryAssembly();
            if (includeAll) // TODO: This would be better with some sort of default configurator that can be changed rather than this bool
            {
                x.AddConsumers(entryAssembly);
            }

            x.AddSagaStateMachines(entryAssembly);
            x.AddSagas(entryAssembly);
            x.AddActivities(entryAssembly);

            massTransitConfig?.Invoke(x);


            switch (messageBusProvider)
            {
                case MessageBusProvider.AzureServiceBus:
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(configuration.GetConnectionString("MessageBus"));
                        cfg.ConfigureEndpoints(context);
                    });
                    break;
                case MessageBusProvider.SqlDb:
                    x.UsingSqlServer((ctx, cfg) =>
                    {
                        cfg.UseSqlMessageScheduler();
                        cfg.ConfigureEndpoints(ctx);
                    });
                    break;
                case MessageBusProvider.RabbitMq:
                    var rabbitMqConfig = configuration.GetRequiredSection("RabbitMq").Get<RabbitMqSettings>();
                    if (rabbitMqConfig is null)
                    {
                        throw new MissingConfigurationException("RabbitMq");
                    }
                    x.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.VirtualHost, h =>
                        {
                            h.Username(rabbitMqConfig.Username);
                            h.Password(rabbitMqConfig.Password);
                        });
                        cfg.ConfigureEndpoints(ctx);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });

        return services;
    }

    private static IServiceCollection ConfigureLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Assembly", assemblyName)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });

        return services;
    }
}