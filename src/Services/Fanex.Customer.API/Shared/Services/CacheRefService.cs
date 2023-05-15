using Fanex.RemotingRef;

namespace Fanex.Customer.API.Shared.Services;

public interface ICacheRefService
{
    string GetProductGroupName(int productGroupId, string language);

    string GetSubProductName(int subGroupId, string language);
}

public class CacheRefService : ICacheRefService
{
    private readonly IRemotingRefService remotingRefService;

    public CacheRefService(IRemotingRefService remotingRefService)
    {
        this.remotingRefService = remotingRefService;
    }

    public string GetProductGroupName(int productGroupId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId("ProductGroup", productGroupId.ToString(), language, string.Empty);

    public string GetSubProductName(int subGroupId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId("SubGroup", subGroupId.ToString(), language, string.Empty);
}