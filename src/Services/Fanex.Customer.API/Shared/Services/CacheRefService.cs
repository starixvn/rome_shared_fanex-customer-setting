using Fanex.RemotingRef;

namespace Fanex.Customer.API.Shared.Services;

public interface ICacheRefService
{
    string GetProductGroupName(int productGroupId, string language);

    string GetSubProductName(int subGroupId, string language);

    string GetSportName(int sportTypeId, string language);

    string GetBetTypeName(int betTypeId, string language);

    string GetGroupTypeName(int groupId, int groupTypeId, int sportTypeId, int betTypeId, string language);
}

public class CacheRefService : ICacheRefService
{
    private readonly IRemotingRefService remotingRefService;

    public CacheRefService(IRemotingRefService remotingRefService)
        => this.remotingRefService = remotingRefService;

    public string GetProductGroupName(int productGroupId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId("ProductGroup", productGroupId.ToString(), language, string.Empty);

    public string GetSubProductName(int subGroupId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId("SubGroup", subGroupId.ToString(), language, string.Empty);

    public string GetSportName(int sportTypeId, string language)
        => remotingRefService.GetSportName(sportTypeId.ToString(), language, string.Empty);

    public string GetBetTypeName(int betTypeId, string language)
        => remotingRefService.GetBetTypeName(betTypeId.ToString(), language);

    public string GetGroupTypeName(int groupId, int groupTypeId, int sportTypeId, int betTypeId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId("G_GT_Sport_Bettype", $"{groupId}_{groupTypeId}_{sportTypeId}_{betTypeId}", language, string.Empty);
}