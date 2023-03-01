using Fanex.Caching;
using Fanex.Logging;
using ONEbook.UM.ThirdParty.AspNetCore.Configurations;
using ONEbook.UM.ThirdParty.AspNetCore.Models;
using System.Text.Json;
namespace ONEbook.UM.ThirdParty.AspNetCore.Services;

public interface IUMService
{
    Task<UnderMaintenanceTime> GetUnderMaintenanceTime(int productId = 0);
}

public class UMService : IUMService
{
    private const string SiteUMCacheKey = "ONEbook.UM.ThirdParty.AspNetCore.Services.UMService.Site.{0}";
    private readonly UMClientConfiguration configuration;
    private readonly ICacheService cacheService;
    private readonly ILogger logger;
    private readonly HttpClient httpClient;

    public UMService(UMClientConfiguration configuration, ICacheService cacheService, ILogger logger, HttpClient httpClient)
    {
        this.configuration = configuration;
        this.cacheService = cacheService;
        this.logger = logger;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri(configuration.ServiceUris);
        this.httpClient.Timeout = TimeSpan.FromSeconds(15000);
    }

    public async Task<UnderMaintenanceTime> GetUnderMaintenanceTime(int productId = 0)
    {
        if (configuration.EnableUnscheduledUM)
        {
            return new UnderMaintenanceTime(true, configuration.UMFrom, configuration.UMTo);
        }

        if (configuration.EnableCheckUM && productId > 0)
        {
            var product = configuration.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null && product.IsUM)
            {
                return new UnderMaintenanceTime(true, product.UMFrom, product.UMTo);
            }

            return await cacheService.GetOrSetAsync(
                string.Format(SiteUMCacheKey, configuration.SiteId),
                () => GetUnderMaintenance(productId),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(configuration.ResultCacheDuration)));
        }

        return new UnderMaintenanceTime(false, "0:00", "0:00");
    }

    private async Task<UnderMaintenanceTime> GetUnderMaintenance(int productId)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/UnderMaintenance/get?siteId={configuration.SiteId}&productId={productId}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<UnderMaintenanceTime>(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
        }

        return new UnderMaintenanceTime(false, "0:00", "0:00");
    }
}