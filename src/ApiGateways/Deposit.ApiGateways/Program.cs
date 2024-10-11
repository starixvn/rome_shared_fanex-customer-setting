using Deposit.ApiGateways;
using Deposit.ApiGateways.DelegatingHandlers;
using Deposit.ApiGateways.Extensions;
using Deposit.ApiGateways.Middlewares;
using Microsoft.AspNetCore.HttpLogging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    var env = builder.Environment;
    builder.Configuration
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
        .AddJsonFile("ocelot.json", true, true)
        .AddJsonFile($"ocelot.{env.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();

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
        .AddOneBookUm(builder.Configuration)
        .AddFanexLogging(builder.Configuration)
        .AddHealthChecks();

    builder.Services.AddControllers();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services
        .AddSwaggerForOcelot(builder.Configuration, (options) =>
        {
            options.GenerateDocsForGatewayItSelf = true;
        })
        .AddOcelot()
        .AddPolly()
        .AddCacheManager(x => { x.WithDictionaryHandle(); })
        .AddDelegatingHandler<ApiKeyHandler>(true)
        .AddDelegatingHandler<NoGzipDelegatingHandler>(true);

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    app.UseHttpLogging();
    app.UseRouting();

    var pathBase = builder.Configuration["PathBase"];
    if (!string.IsNullOrEmpty(pathBase))
    {
        app.UsePathBase(pathBase);
    }

    app.MapHealthChecks("/healthz");
    app.MapGet("/", () => "Hello, I'm Dep Api!");
    app.UseEndpoints(_ => { });

    app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), builder => builder.UseMiddleware<UMMiddleware>());

    app.UseSwaggerForOcelotUI(opt =>
        {
            if (!string.IsNullOrEmpty(pathBase))
            {
                opt.DownstreamSwaggerEndPointBasePath = $"{pathBase}/swagger/docs";
            }
            opt.PathToSwaggerGenerator = "/swagger/docs";
        })
        .UseOcelot()
        .Wait();
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


