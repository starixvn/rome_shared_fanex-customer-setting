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

builder.Services.AddHealthChecks();

builder.Services
    .AddOcelot();

var app = builder.Build();
app.UseRouting();

app.MapHealthChecks("/heathz");
app.MapGet("/", () => "Hello world!");
app.UseEndpoints(_ => { });
app.UseOcelot().Wait();
app.Run();
