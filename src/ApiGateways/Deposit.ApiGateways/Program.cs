using Deposit.ApiGateways;
using Deposit.ApiGateways.Extensions;
using Deposit.ApiGateways.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
    .AddJsonFile("ocelot.json", true, true)
    .AddJsonFile($"ocelot.{env.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services
    .AddOneBookUm(builder.Configuration)
    .AddHealthChecks();

builder.Services.AddControllers();

builder.Services
    .AddSwaggerForOcelot(builder.Configuration, (options) =>
    {
        options.GenerateDocsForGatewayItSelf = true;
    })
    .AddOcelot()
    .AddDelegatingHandler<ApiKeyHandler>(true);

var app = builder.Build();
app.UseRouting();

app.MapHealthChecks("/healthz");
app.MapGet("/", () => "Hello, I'm Dep Api!");
app.UseEndpoints(_ => { });

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), builder => builder.UseMiddleware<UMMiddleware>());

app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
    })
    .UseOcelot()
    .Wait();
if (!app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}
app.Run();
