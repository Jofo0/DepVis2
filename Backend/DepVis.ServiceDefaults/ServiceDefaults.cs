using System.Reflection;
using MassTransit;
using Microsoft.Data.SqlClient;
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
    public static IHostApplicationBuilder AddServiceDefaults(
        this IHostApplicationBuilder builder,
        string databaseConnectionString,
        bool createMassTransitInfra = false
    )
    {
        EnsureDatabaseExists(databaseConnectionString);
        builder.Services.ConfigureMassTransit(
            builder.Configuration,
            createInfra: createMassTransitInfra
        );

        return builder;
    }

    private static void EnsureDatabaseExists(string fullConnectionString, int maxRetries = 10)
    {
        var builder = new SqlConnectionStringBuilder(fullConnectionString);

        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException(
                "Connection string must include Initial Catalog / Database."
            );

        builder.InitialCatalog = "master";
        var masterConnectionString = builder.ConnectionString;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Console.WriteLine(
                    $"[DB INIT] Attempt {attempt}: connecting to SQL Server (master)..."
                );

                using var connection = new SqlConnection(masterConnectionString);
                connection.Open();

                var cmdText =
                    $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                    BEGIN
                        PRINT 'Creating database {databaseName}';
                        CREATE DATABASE [{databaseName}];
                    END";

                using var command = new SqlCommand(cmdText, connection);
                command.ExecuteNonQuery();

                Console.WriteLine($"[DB INIT] Database '{databaseName}' ready.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[DB INIT] Failed (attempt {attempt}/{maxRetries}): {ex.Message}"
                );

                if (attempt == maxRetries)
                    throw;

                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
        }
    }

    private static IServiceCollection ConfigureMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? massTransitConfig = null,
        bool includeAll = true,
        bool createInfra = false
    )
    {
        Log.Information("Configuring MassTransit");

        var sqlConnectionString =
            configuration.GetConnectionString("Database")
            ?? throw new Exception("Database ConnectionString is not set.");

        services
            .AddOptions<SqlTransportOptions>()
            .Configure(options =>
            {
                options.ConnectionString = sqlConnectionString;
            });

        services.AddSqlServerMigrationHostedService(x =>
        {
            x.CreateDatabase = false;
            x.CreateInfrastructure = createInfra;
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
