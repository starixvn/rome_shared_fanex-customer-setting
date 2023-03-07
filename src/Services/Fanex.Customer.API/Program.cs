using Fanex.Caching;
using Fanex.Customer.API;
using Fanex.Customer.API.Extensions;
using Fanex.Customer.API.Modules.CopyCustomer;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.SecurityCode;
using Fanex.Data.Repository;
using Fanex.RemotingRef;
using MinimalApi.Endpoint.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddFanexLogging(builder.Configuration);
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IRemotingRefService, RemotingRefService>();
builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.RegisterSecurityCodeModule();
builder.Services.RegisterCustomerModule();
builder.Services.RegisterCopyCustomerModule(builder.Configuration);
builder.Services.AddEndpoints();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
});
app.UseFanexData(builder.Configuration, app.Environment);
app.UseRemotingRef(builder.Configuration);
app.ConfigureExceptionHandler(Fanex.Logging.Logger.Log);

app.MapEndpoints();
app.MapHealthChecks("/healthz");

if (!app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.Run();