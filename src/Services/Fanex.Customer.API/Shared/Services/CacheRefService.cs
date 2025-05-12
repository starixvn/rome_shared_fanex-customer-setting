using Fanex.Customer.API.Shared.Constants;
using Fanex.RemotingRef;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Shared.Services;

public interface ICacheRefService
{
    string GetProductName(int productId, string language = "en-US", bool isSiteLabel777 = false);

    string GetProductGroupName(int productGroupId, string language = "en-US");

    string GetSubGroupName(int? subGroupId, string language = "en-US");

    string GetSportTypeName(int sporttypeId, string language = "en-US");

    string GetBetTypeName(int bettypeId, string language = "en-US");

    string GetDisplayedBetTypeName(int bettypeId, string language = "en-US");

    string GetSportsbookPotentialWinLimitName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US");
    string GetSportsbookCommSMAName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US");

    string GetSportsbookCommMemberName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US");

    string GetNameByGroupSubGroup(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US", bool isSiteLabel777 = false);

    string GetDisplayNameByDisplayType(
        int sportTypeId,
        int betTypeId,
        int productGroupId,
        int groupTypeId,
        int displayType,
        int roleId,
        bool? isDirectMember = null,
        string language = "en-US",
        bool isSiteLabel777 = false);

    string GetParlayMaxPayoutBetSettingInfoSportName(string settingId, bool isSpecialPlatform, string language);
}

public class CacheRefService : ICacheRefService
{
    private readonly IRemotingRefService remotingRef;

    private const string ProductRefId = "Product";

    private const string ProductUATRefId = "Product_UAT";

    private const string ProductGroupRefId = "ProductGroup";

    private const string DummySportRefId = "DummySport";

    private const string DummySportUATRefId = "DummySport_UAT";

    private const string SubGroupRefId = "SubGroup";

    private const string SB_PotentialWinLimit_RefId = "SB_PotentialWinLimit";
    private const string SB_Comm_SMA_RefId = "SB_Comm_SMA";

    private const string SB_Comm_Member_RefId = "SB_Comm_Member";

    private const string G_GT_Sport_Bettype_RefId = "G_GT_Sport_Bettype";

    private const string G_GT_Sport_Bettype_RefId_Saba = "G_GT_Sport_BT_Saba";

    private Dictionary<string, string> cacheData = new Dictionary<string, string>();

    private readonly AppSettings _appSettings;

    public CacheRefService(IRemotingRefService remotingRef, IOptions<AppSettings> appSettings)
    {
        this.remotingRef = remotingRef;
        _appSettings = appSettings.Value;
    }

    public string GetProductName(int productId, string language = "en-US", bool isSiteLabel777 = false)
    {
        string name = null;
        if (isSiteLabel777)
        {
            name = remotingRef.GetResourceDataByRefIdAndResourceId($"{ProductRefId}_Saba", productId.ToString(), language, string.Empty);

            return string.IsNullOrEmpty(name) ?
                remotingRef.GetResourceDataByRefIdAndResourceId(ProductRefId, productId.ToString(), language, productId.ToString()) :
                name;
        }

        if (_appSettings.SettingStatus != 0)
        {
            name = remotingRef.GetResourceDataByRefIdAndResourceId(ProductUATRefId, productId.ToString(), language, string.Empty);
        }

        return string.IsNullOrEmpty(name) ?
            remotingRef.GetResourceDataByRefIdAndResourceId(ProductRefId, productId.ToString(), language, productId.ToString()) :
            name;
    }

    public string GetProductGroupName(int productGroupId, string language = "en-US")
        => remotingRef.GetResourceDataByRefIdAndResourceId(ProductGroupRefId, productGroupId.ToString(), language, productGroupId.ToString());

    public string GetSubGroupName(int? subGroupId, string language = "en-US")
        => remotingRef.GetResourceDataByRefIdAndResourceId(SubGroupRefId, subGroupId?.ToString(), language, subGroupId?.ToString());

    public string GetSportTypeName(int sportTypeId, string language = "en-US")
    {
        var sportTypeName = remotingRef.GetSportName(sportTypeId.ToString(), language);
        if (string.IsNullOrWhiteSpace(sportTypeName))
        {
            if (_appSettings.SettingStatus != 0)
            {
                sportTypeName = remotingRef.GetResourceDataByRefIdAndResourceId(
                DummySportUATRefId,
                sportTypeId.ToString(),
                language,
                string.Empty);
            }

            sportTypeName = string.IsNullOrEmpty(sportTypeName) ? remotingRef.GetResourceDataByRefIdAndResourceId(
                DummySportRefId,
                sportTypeId.ToString(),
                language,
                sportTypeId.ToString())
                : sportTypeName;
        }

        return sportTypeName;
    }

    public string GetBetTypeName(int bettypeId, string language = "en-US")
        => remotingRef.GetBetTypeName(bettypeId.ToString(), language);

    public string GetDisplayedBetTypeName(int bettypeId, string language = "en-US")
        => remotingRef.GetDisplayedBetTypeName(bettypeId.ToString(), language);

    public string GetSportsbookPotentialWinLimitName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US")
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        return GetOrSetFromCacheData(SB_PotentialWinLimit_RefId, refKey, language, refKey);
    }

    public string GetSportsbookCommSMAName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US")
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        return GetOrSetFromCacheData(SB_Comm_SMA_RefId, refKey, language, refKey);
    }

    public string GetSportsbookCommMemberName(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US")
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        return GetOrSetFromCacheData(SB_Comm_Member_RefId, refKey, language, refKey);
    }

    public string GetNameByGroupSubGroup(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId, string language = "en-US", bool isSiteLabel777 = false)
    {
        var refKey = CombineRefKey(productGroupId, groupTypeId, sporttypeId, bettypeId);
        var value = isSiteLabel777 ? GetOrSetFromCacheData(G_GT_Sport_Bettype_RefId_Saba, refKey, language, string.Empty) : string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            value = GetOrSetFromCacheData(G_GT_Sport_Bettype_RefId, refKey, language, string.Empty);
        }

        return value;
    }

    public string GetDisplayNameByDisplayType(
        int sportTypeId,
        int betTypeId,
        int productGroupId,
        int groupTypeId,
        int displayType,
        int roleId,
        bool? isDirectMember = null,
        string language = "en-US",
        bool isSiteLabel777 = false)
    {
        if (displayType == DisplayType.CommSB)
        {
            if (roleId == CustomerLevel.Member || isDirectMember.GetValueOrDefault())
            {
                return GetSportsbookCommMemberName(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
            }
            else
            {
                return GetSportsbookCommSMAName(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language);
            }
        }
        else if (displayType == DisplayType.PotentialWinLimit)
        {
            return GetSportsbookPotentialWinLimitName(
                productGroupId,
                groupTypeId,
                sportTypeId,
                betTypeId,
                language);
        }
        else
        {
            var displayName = GetNameByGroupSubGroup(
                    productGroupId,
                    groupTypeId,
                    sportTypeId,
                    betTypeId,
                    language,
                    isSiteLabel777);
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = GetSportTypeName(sportTypeId, language);
            }

            return displayName;
        }
    }

    public string GetParlayMaxPayoutBetSettingInfoSportName(string settingId, bool isSpecialPlatform, string language)
    {
        return remotingRef.GetResourceDataByRefIdAndResourceId(
                    isSpecialPlatform ? "BetSettingRef_Saba" : "BetSettingRef",
                    settingId,
                    language,
                    settingId);
    }

    private string CombineRefKey(int productGroupId, int groupTypeId, int sporttypeId, int bettypeId)
        => $"{productGroupId}_{groupTypeId}_{sporttypeId}_{bettypeId}";

    private string GetOrSetFromCacheData(string refId, string refKey, string language, string defaultValue)
    {
        if (cacheData.ContainsKey($"{refId}_{refKey}_{language}"))
        {
            return cacheData[$"{refId}_{refKey}_{language}"];
        }
        else
        {
            var refVal = remotingRef.GetResourceDataByRefIdAndResourceId(refId, refKey, language, defaultValue);
            cacheData.Add($"{refId}_{refKey}_{language}", refVal);
            return refVal;
        }
    }
}