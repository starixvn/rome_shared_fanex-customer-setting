// // This application entry point is based on ASP.NET Core new project templates and is included
// // as a starting point for app host configuration.
// // This file may need updated according to the specific scenario of the application being upgraded.
// // For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

// using System.Diagnostics.CodeAnalysis;
// using Fanex.Logging;
// using Fanex.Logging.Extensions;
// using Fanex.Logging.Extensions.AspNetCore;
// using Fanex.Logging.Extensions.Database;
// using Fanex.Logging.RabbitMQ;

// namespace Alpha.CloneAccount.Api;

// [ExcludeFromCodeCoverage]
// public class Program
// {
//     public static void Main(string[] args)
//     {
//         var configuration = GetConfiguration();
//         InitFanexLogger(configuration);
//         var appName = typeof(Program).Assembly.GetName().Name;

//         try
//         {
//             var host = CreateHostBuilder(configuration, args).Build();
//             Logger.Log.Info($"Starting {appName}");
//             host.Run();
//         }
//         catch (Exception ex)
//         {
//             Logger.Log.Error($"Program terminated unexpectedly {appName}!", ex);
//         }
//     }

//     static IConfiguration GetConfiguration()
//     {
//         var builder = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//             .AddEnvironmentVariables();

//         return builder.Build();
//     }

//     static void InitFanexLogger(IConfiguration configuration)
//     {
//         LogManager
//             .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
//             .Use(new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"]));

//         ExceptionHandler
//             .UseConfig(new LoggingConfiguration()
//                 .GetHttpHeaderInfo()
//                 .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo));
//     }

//     public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
//         Host.CreateDefaultBuilder(args)
//             .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
//             .ConfigureWebHostDefaults(webBuilder =>
//             {
//                 webBuilder.UseStartup<Startup>();
//                 webBuilder.UseUrls("http://0.0.0.0:5000");
//             });
// }

