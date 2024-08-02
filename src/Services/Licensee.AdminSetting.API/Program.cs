using Fanex.Data.SqlServer;
using Licensee.AdminSetting.API;
using Licensee.AdminSetting.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddFanexLogging(builder.Configuration)
    .AddFanexSql(builder.Configuration)
    .AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "LicenseeAdminSettingAPI";
    config.Title = "LicenseeAdminSettingAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "LicenseeAdminSettingAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/", () => "Hello World!");

app.Run();
