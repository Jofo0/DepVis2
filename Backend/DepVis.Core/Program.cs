using DepVis.Core.Context;
using DepVis.Core.Repositories;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services;
using DepVis.Core.Services.Interfaces;
using DepVis.Core.Services.Processing;
using DepVis.ServiceDefaults;
using DepVis.Shared.Options;
using DepVis.Shared.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new Exception("Database ConnectionString is not set.");

builder
    .Services.AddControllers()
    .AddOData()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
builder.Services.AddOpenApi();

builder.Services.AddScoped<MinioStorageService>();
builder.Services.AddScoped<IGitService, GitService>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IGraphService, GraphService>();
builder.Services.AddScoped<IVulnerabilityService, VulnerabilityService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IProjectBranchService, ProjectBranchService>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectBranchRepository, ProjectBranchRepository>();
builder.Services.AddScoped<ISbomRepository, SbomRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IVulnerabilityRepository, VulnerabilityRepository>();

builder.Services.AddScoped<ISbomIngestionOrchestrator, SbomIngestionOrchestrator>();
builder.Services.AddScoped<ISbomProcessor, SbomProcessor>();
builder.Services.AddScoped<ICycloneDxBomLoader, CycloneDxBomLoader>();
builder.Services.AddScoped<IVulnerabilityIngestionService, VulnerabilityIngestionService>();
builder.Services.AddScoped<ISbomPackageBuilder, SbomPackageBuilder>();
builder.Services.AddScoped<IDependencyGraphBuilder, DependencyGraphBuilder>();
builder.Services.AddScoped<IPackageVulnerabilityMapper, PackageVulnerabilityMapper>();

builder.Services.AddDbContext<DepVisDbContext>(options =>
    options.UseSqlServer(
        dbConnectionString,
        providerOptions => providerOptions.EnableRetryOnFailure()
    )
);

builder.AddServiceDefaults(dbConnectionString, createMassTransitInfra: true);

var frontendCors = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        frontendCors,
        policy =>
        {
            policy
                .WithOrigins(
                    builder.Configuration.GetConnectionString("FrontEnd")
                        ?? throw new Exception("FrontEnd ConnectionString is not set.")
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});


builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection(key: nameof(ConnectionStrings))
);

var app = builder.Build();
app.UseCors(frontendCors);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DepVisDbContext>();
    db.Database.Migrate();
}

app.Run();
