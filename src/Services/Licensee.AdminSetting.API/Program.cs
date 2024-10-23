using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Licensee.AdminSetting.API;
using Licensee.AdminSetting.API.Extensions;
using Licensee.AdminSetting.API.Features.Accounts;
using Licensee.AdminSetting.API.Features.Licensees;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using Licensee.AdminSetting.API.Features.UserLog;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var env = builder.Environment;
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Filter.ByExcluding("RequestPath like '/health%'")
        .WriteTo.Console()
        .Enrich.WithProperty("Team", "Rome")
        .Enrich.WithProperty("Application", env.ApplicationName)
        .Enrich.WithProperty("Environment", builder.Configuration["Environment"]));

    builder.Services.AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
        logging.RequestHeaders.Add("sec-ch-ua");
        logging.RequestHeaders.Add("x-forwarded-for");
        logging.MediaTypeOptions.AddText("application/javascript");
        logging.RequestBodyLogLimit = 4096;
        logging.ResponseBodyLogLimit = 4096;
        logging.CombineLogs = true;
    });

    builder.Services
        .AddFanexLogging(builder.Configuration)
        .AddFanexSql(builder.Configuration)
        .AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
    builder.Services.AddSingleton<IUserLogHandler, UserLogHandler>();
    builder.Services.AddSingleton<ISiteDBHandler, SiteDBHandler>();
    builder.Services.AddSingleton<ILicenseesHandler, LicenseesHandler>();
    builder.Services.AddSingleton<ICurrencyHandler, CurrencyHandler>();
    builder.Services.AddSingleton<IAccountsHandler, AccountsHandler>();

    builder.Services.AddHealthChecks();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Licensee Admin Setting API", Version = "v1" });
    });

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("v1/swagger.json", "Licensee Admin Setting API V1");
    });

    app.MapGet("/", () => "Hello World!");
    app.MapHealthChecks("/healthz");
    app.RegisterLicenseesEndpoints();
    app.RegisterAccountsEndpoints();

    if (!app.Environment.IsDevelopment())
    {
        builder.WebHost.UseUrls("http://0.0.0.0:5000");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}