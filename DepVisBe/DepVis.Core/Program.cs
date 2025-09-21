using DepVis.Core.Context;
using DepVis.Core.Repositories;
using DepVis.Core.Repositories.Interfaces;
using DepVis.Core.Services;
using DepVis.Core.Services.Interfaces;
using DepVis.ServiceDefaults;
using DepVis.Shared.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
;
builder.Services.AddOpenApi();

builder.Services.AddScoped<MinioStorageService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddDbContext<DepVisDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Database")
            ?? throw new Exception("Database ConnectionString is not set.")
    )
);

builder.AddServiceDefaults();

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
