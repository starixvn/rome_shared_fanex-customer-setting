// // This Startup file is based on ASP.NET Core new project templates and is included
// // as a starting point for DI registration and HTTP request processing pipeline configuration.
// // This file will need updated according to the specific scenario of the application being upgraded.
// // For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

// using Fanex.Logging;
// using MediatR;
// using Microsoft.OpenApi.Models;
// using Alpha.CloneAccount.Api.Extensions;
// using Fanex.Data.Repository;
// using Alpha.CloneAccount.Repositories;
// using Alpha.CloneAccount.Services;
// using Alpha.CloneAccount.Configurations;
// using Alpha.CloneAccount.Helpers;
// using Fanex.RemotingRef;
// using ONEbook.NG2.API.Client.Extensions;
// using Fanex.Caching;
// using ONEbook.UM.ThirdParty.AspNetCore.Extensions;

// namespace Alpha.CloneAccount.Api;

// public class Startup
// {
//     // public Startup(IConfiguration configuration)
//     // {
//     //     Configuration = configuration;
//     // }

//     // public IConfiguration Configuration { get; }

//     // // This method gets called by the runtime. Use this method to add services to the container.
//     // public void ConfigureServices(IServiceCollection services)
//     // {
//     //     services.AddControllers();

//     //     services.AddMediatR(typeof(Startup));

//     //     var appSettings = new AppSettings();
//     //     Configuration.GetSection(AppSettings.Setting).Bind(appSettings);
//     //     services.AddSingleton(appSettings);

//     //     var kenoConfiguration = new KenoConfiguration();
//     //     Configuration.GetSection(KenoConfiguration.Keno).Bind(kenoConfiguration);
//     //     services.AddSingleton(kenoConfiguration);

//     //     services.AddSingleton(Logger.Log);
//     //     services.AddSingleton<ICacheService, CacheService>();
//     //     services.AddSingleton<IRemotingRefService, RemotingRefService>();
//     //     services.AddSingleton<IDynamicRepository, DynamicRepository>();
//     //     services.AddSingleton<IMessageHelper, MessageHelper>();
//     //     services.AddSingleton<IKenoRepository, KenoRepository>();
//     //     services.AddScoped<CustomerService, CustomerServiceImpl>();
//     //     services.AddSingleton<PasswordService, PasswordServiceImpl>();
//     //     services.AddSingleton<SportbooksCopyMemberSpParamsBuilder, SportbooksCopyMemberSpParamsBuilderImpl>();
//     //     services.AddSingleton<DCSService, DCSServiceImpl>();
//     //     services.AddSingleton<KenoService, KenoServiceImpl>();
//     //     services.AddONEbookUMThirdParty(Configuration);
//     //     services.AddONEbookNG2APIClient(Configuration);
//     //     services.AddSingleton<IAPILogger, APILogger>();
//     //     services.AddSingleton<NG2Service, NG2ServiceImpl>();
//     //     services.AddScoped<ThirdPartyProductService, ThirdPartyProductServiceImpl>();


//     //     services.AddEndpointsApiExplorer();
//     //     services.AddSwaggerGen(options =>
//     //     {
//     //         options.SwaggerDoc("v1", new OpenApiInfo { Title = "Alpha Clone Account API", Version = "v1" });
//     //         options.CustomSchemaIds(type => type.FullName);
//     //         var xmlFilename = $"{typeof(Startup).Assembly.GetName().Name}.xml";
//     //         options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
//     //     });
//     // }

//     // // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//     // public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//     // {
//     //     if (env.IsDevelopment())
//     //     {
//     //         app.UseDeveloperExceptionPage();
//     //     }
//     //     else
//     //     {
//     //         app.UseHsts();
//     //         app.ConfigureExceptionHandler(Logger.Log);
//     //     }

//     //     app.UseRouting();
//     //     app.UseFanexData(Configuration, env);
//     //     app.UseRemotingRef(Configuration);

//     //     app.UseEndpoints(endpoints =>
//     //     {
//     //         endpoints.MapGet("/", () => "Alpha Clone Account API!!!");
//     //         endpoints.MapControllerRoute(
//     //             name: "default",
//     //             pattern: "{controller=Home}/{action=Index}/{id?}");
//     //     });

//     //     app.UseSwagger();
//     //     app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ONEBookUM v1"));
//     // }
// }
