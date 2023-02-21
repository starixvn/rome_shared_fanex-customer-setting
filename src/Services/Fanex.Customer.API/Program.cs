using Fanex.Customer.API.Extensions;
using Fanex.Customer.API.Modules.SecurityCode;
using Fanex.Data.Repository;
using MinimalApi.Endpoint.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.RegisterSecurityCodeModule();
builder.Services.AddEndpoints();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseFanexData(builder.Configuration, app.Environment);

app.MapEndpoints();
app.MapHealthChecks("/healthz");

if (!app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.Run();