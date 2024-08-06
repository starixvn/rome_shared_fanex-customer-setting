using System.Net;
using Fanex.Caching;
using Fanex.Customer.API;
using Fanex.Customer.API.Extensions;
using Fanex.Customer.API.Middlewares;
using Fanex.Customer.API.Modules.CopyCustomer;
using Fanex.Customer.API.Modules.Rules;
using Fanex.Customer.API.Modules.CreateCustomer;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.SecurityCode;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Fanex.RemotingRef;
using MinimalApi.Endpoint.Extensions;
using ONEbook.OTP.Services;
using TelegramBot.Client.Services;
using SophosOtp = ONEbook.OTP.Services.OtpService;
using Fanex.Customer.API.Modules.Balance;
using Fanex.Customer.API.Modules.EditCustomer;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddFanexLogging(builder.Configuration);
builder.Services.AddFanexRemotingRef(builder.Configuration);
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IRemotingRefService, RemotingRefService>();
builder.Services.AddSingleton<ICacheRefService, CacheRefService>();
builder.Services.AddSingleton<IDynamicRepository, DynamicRepository>();
builder.Services.AddSingleton<ISiteDBGroupService, SiteDBGroupService>();
builder.Services.AddSingleton<ISiteDBService, SiteDBService>();
builder.Services.AddSingleton<IScaleoutDBService, ScaleoutDBService>();
builder.Services.AddSingleton<IBalanceService, BalanceService>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<ISettingProfileService, SettingProfileService>();
builder.Services.AddSingleton<IUMService, UMService>();
builder.Services.AddSingleton<ICustomerSettingHelper, CustomerSettingHelper>();
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
builder.Services.RegisterCreateCustomerModule();
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

//if (!app.Environment.IsDevelopment())
//{
    builder.WebHost.UseUrls("http://localhost:6001");
//}

app.Run();