using Fanex.RemotingRef;

namespace Fanex.Customer.API.Shared.Services;

public interface ICacheRefService
{
    string GetProductGroupName(int productGroupId, string language = "en-US");

    string GetSubGroupName(int? subGroupId, string language = "en-US");

    string GetSportTypeName(int sporttypeId, string language = "en-US");

    string GetBetTypeName(int bettypeId, string language = "en-US");

    string GetSportsbookCommSMAName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US");

    string GetSportsbookCommMemberName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US");

    string GetNameByGroupSubGroup(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US");
}

public class CacheRefService : ICacheRefService
{
    private readonly IRemotingRefService remotingRef;

    private const string ProductRefId = "Product";

    private const string ProductGroupRefId = "ProductGroup";

    private const string DummySportRefId = "DummySport";

    private const string SubGroupRefId = "SubGroup";

    private const string SB_Comm_SMA_RefId = "SB_Comm_SMA";

    private const string SB_Comm_Member_RefId = "SB_Comm_Member";

    private const string G_GT_Sport_Bettype_RefId = "G_GT_Sport_Bettype";

    public CacheRefService(IRemotingRefService remotingRef)
    {
        this.remotingRef = remotingRef;
    }

    public string GetProductName(int productId, string language = "en-US")
        => remotingRef.GetResourceDataByRefIdAndResourceId(ProductRefId, productId.ToString(), language, productId.ToString());

    public string GetProductGroupName(int productGroupId, string language = "en-US")
        => remotingRef.GetResourceDataByRefIdAndResourceId(ProductGroupRefId, productGroupId.ToString(), language, productGroupId.ToString());

    public string GetSubGroupName(int? subGroupId, string language = "en-US")
        => remotingRef.GetResourceDataByRefIdAndResourceId(SubGroupRefId, subGroupId?.ToString(), language, subGroupId?.ToString());

    public string GetSportTypeName(int sportTypeId, string language = "en-US")
    {
        var sportTypeName = remotingRef.GetSportName(sportTypeId.ToString(), language);
        if (string.IsNullOrWhiteSpace(sportTypeName))
        {
            sportTypeName = remotingRef.GetResourceDataByRefIdAndResourceId(
                DummySportRefId,
                sportTypeId.ToString(),
                language,
                string.Empty);
        }

        return sportTypeName;
    }

    public string GetBetTypeName(int bettypeId, string language = "en-US")
        => remotingRef.GetBetTypeName(bettypeId.ToString(), language);

    public string GetSportsbookCommSMAName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US")
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        return remotingRef.GetResourceDataByRefIdAndResourceId(SB_Comm_SMA_RefId, refKey, language, refKey);
    }

    public string GetSportsbookCommMemberName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US")
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        return remotingRef.GetResourceDataByRefIdAndResourceId(SB_Comm_Member_RefId, refKey, language, refKey);
    }

    public string GetNameByGroupSubGroup(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US")
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        return remotingRef.GetResourceDataByRefIdAndResourceId(G_GT_Sport_Bettype_RefId, refKey, language, refKey);
    }

    private static string CombineRefKey(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId)
        => $"{productGroupId}_{groupTypeId}_{sporttypeId}_{bettypeId}";
}