using Fanex.Data.SqlServer;
using Licensee.AdminSetting.API;
using Licensee.AdminSetting.API.Extensions;
using Licensee.AdminSetting.API.Features.Licensees;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFanexLogging(builder.Configuration)
    .AddFanexSql(builder.Configuration)
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSingleton<ISiteDBHandler, SiteDBHandler>();
builder.Services.AddSingleton<ILicenseesHandler, LicenseesHandler>();

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Licensee Admin Setting API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.SwaggerEndpoint("v1/swagger.json", "Licensee Admin Setting API V1");
});

app.MapGet("/", () => "Hello World!");
app.MapHealthChecks("/healthz");
app.RegisterLicenseesEndpoints();

if (!app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.Run();
