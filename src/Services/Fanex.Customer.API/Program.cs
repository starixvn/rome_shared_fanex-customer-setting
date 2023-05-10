using System.Net;
using Fanex.Caching;
using Fanex.Customer.API;
using Fanex.Customer.API.Extensions;
using Fanex.Customer.API.Helpers;
using Fanex.Customer.API.Middlewares;
using Fanex.Customer.API.Modules.CopyCustomer;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Rules;
using Fanex.Customer.API.Modules.CreateCustomer;
using Fanex.Customer.API.Modules.SecurityCode;
using Fanex.Data.Repository;
using MinimalApi.Endpoint.Extensions;
using ONEbook.OTP.Services;
using TelegramBot.Client.Services;
using SophosOtp = ONEbook.OTP.Services.OtpService;
using Fanex.Customer.API.Modules.PositionTakingTemplate;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor()
                .AddSingleton<IShardInfoAccessor, ShardInfoAccessor>();
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddFanexLogging(builder.Configuration);
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.AddHttpClient<ISettingService, SettingService>(httpClient =>
{
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    httpClient.BaseAddress = new Uri(builder.Configuration["TelegramClientUrl"]);
    httpClient.DefaultRequestHeaders.Add("Accept-Language", Thread.CurrentThread.CurrentCulture.Name);
});
builder.Services.AddHttpClient<IOTPService, OTPService>(httpClient =>
{
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    httpClient.BaseAddress = new Uri(builder.Configuration["TelegramClientUrl"]);
    httpClient.DefaultRequestHeaders.Add("Accept-Language", Thread.CurrentThread.CurrentCulture.Name);
});
builder.Services.AddScoped<SophosOtp, OtpServiceImpl>();
builder.Services.RegisterSecurityCodeModule();
builder.Services.RegisterCustomerModule();
builder.Services.RegisterCopyCustomerModule(builder.Configuration);
builder.Services.RegisterRulesServiceModule();
builder.Services.RegisterLiteModeModule();
builder.Services.RegisterPositionTemplateModule();
builder.Services.AddEndpoints();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("./swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});
app.UseFanexData(builder.Configuration, app.Environment, builder.Services);

app.UseMiddleware<ExceptionMiddleware>();

app.MapEndpoints();
app.MapHealthChecks("/healthz");

if (!app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.Run();