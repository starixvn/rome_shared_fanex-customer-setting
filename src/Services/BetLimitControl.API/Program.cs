using BetLimitControl.API;
using BetLimitControl.API.Authentication;
using BetLimitControl.API.Extensions;
using BetLimitControl.API.Features.MaxBetControl;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFanexSql(builder.Configuration);
builder.Services.AddFanexLogging(builder.Configuration);

builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.AddSingleton<IMaxBetControlService, MaxBetControlService>();
builder.Services.AddTransient<IApiKeyValidation, ApiKeyValidation>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
{
    document.DocumentName = "BetLimitControlAPI";
    document.Title = "BetLimitControlAPI v1";
    document.Version = "v1";
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "BetLimitControlAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
    });
}

app.MapHealthChecks("/healthz").AllowAnonymous();

app.UseMiddleware<ApiKeyMiddleware>();

app.RegisterMaxBetControlEndpoints();

app.Run();
