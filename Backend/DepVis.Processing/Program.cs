using DepVis.SbomProcessing;
using DepVis.SbomProcessing.Options;
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

builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection(key: nameof(ConnectionStrings))
);

builder.Services.Configure<ProcessingOptions>(
    builder.Configuration.GetSection("Processing")
);


var app = builder.Build();

app.Run();
