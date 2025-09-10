using DepVis.ServiceDefaults;
using DepVis.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddScoped<MinioStorageService>();

var app = builder.Build();

app.Run();
