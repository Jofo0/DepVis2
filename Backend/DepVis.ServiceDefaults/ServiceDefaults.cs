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

    private static void EnsureDatabaseExists(string fullConnectionString, int slowBoundary = 10)
    {
        var totalBoundary = slowBoundary * 2;
        var builder = new SqlConnectionStringBuilder(fullConnectionString);

        var databaseName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException(
                "Connection string must include Initial Catalog / Database."
            );

        builder.InitialCatalog = "master";
        var masterConnectionString = builder.ConnectionString;

        for (int attempt = 1; attempt <= totalBoundary; attempt++)
        {
            try
            {
                Console.WriteLine(
                    $"[DB INIT] Attempt {attempt}: connecting to SQL Server (master)..."
                );

                using var masterConnection = new SqlConnection(masterConnectionString);
                masterConnection.Open();

                // 1) Create DB if it does not exist
                using (var createCommand = masterConnection.CreateCommand())
                {
                    createCommand.CommandText =
                        $@"
IF DB_ID(@dbName) IS NULL
BEGIN
    DECLARE @sql nvarchar(max) =
        N'CREATE DATABASE ' + QUOTENAME(@dbName);
    EXEC(@sql);
END";
                    createCommand.Parameters.AddWithValue("@dbName", databaseName);
                    createCommand.ExecuteNonQuery();
                }

                // 2) Wait until SQL Server reports the DB is ONLINE
                var isOnline = false;

                for (var i = 0; i < 30; i++)
                {
                    using var statusCommand = masterConnection.CreateCommand();
                    statusCommand.CommandText =
                        @"SELECT state_desc FROM sys.databases WHERE name = @dbName";
                    statusCommand.Parameters.AddWithValue("@dbName", databaseName);

                    var state = statusCommand.ExecuteScalar() as string;
                    Console.WriteLine(
                        $"[DB INIT] Database '{databaseName}' state: {state ?? "<null>"}"
                    );

                    if (string.Equals(state, "ONLINE", StringComparison.OrdinalIgnoreCase))
                    {
                        isOnline = true;
                        break;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }

                if (!isOnline)
                    throw new InvalidOperationException(
                        $"Database '{databaseName}' was created/found, but never became ONLINE."
                    );

                // 3) Verify actual usability by connecting to the target DB
                using var dbConnection = new SqlConnection(fullConnectionString);
                dbConnection.Open();

                using var pingCommand = dbConnection.CreateCommand();
                pingCommand.CommandText = "SELECT 1";
                pingCommand.ExecuteScalar();

                Console.WriteLine($"[DB INIT] Database '{databaseName}' is ready.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[DB INIT] Failed (attempt {attempt}/{totalBoundary}): {ex.Message}"
                );

                Thread.Sleep(
                    attempt >= slowBoundary ? TimeSpan.FromSeconds(25) : TimeSpan.FromSeconds(3)
                );
            }
        }

        throw new InvalidOperationException(
            $"Database '{databaseName}' did not become ready in time."
        );
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
