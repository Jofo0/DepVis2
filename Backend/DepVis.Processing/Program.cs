using DepVis.SbomProcessing;
using DepVis.ServiceDefaults;
using DepVis.Shared.Options;
using DepVis.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
var dbConnectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new Exception("Database ConnectionString is not set.");

builder.AddServiceDefaults(dbConnectionString);
builder.Services.AddScoped<MinioStorageService>();
builder.Services.AddScoped<ProcessingService>();

// TODO REMOVE DUPLICIT

builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection(key: nameof(ConnectionStrings))
);


var app = builder.Build();

app.Run();
