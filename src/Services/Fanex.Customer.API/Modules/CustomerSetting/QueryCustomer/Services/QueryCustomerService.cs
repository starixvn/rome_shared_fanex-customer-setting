using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.EditSettings;
using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using static Fanex.Customer.API.Modules.CustomerSetting.Shared.Services.SettingProfileService;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria;

namespace Fanex.Customer.API.Modules.QueryCustomer.Services;

public interface IQueryCustomerService
{
    Task<CustomerSettingModel> QuerySuper(QuerySuperRequest request);

    Task<CustomerSettingModel> Query(QueryRequest request);
}

public class QueryCustomerService : BaseQueryCustomerService, IQueryCustomerService
{
    private readonly ICustomerService customerService;
    private readonly ISiteDBGroupService siteDBGroupService;

    public QueryCustomerService(
        ICustomerService customerService,
        ISiteDBGroupService siteDBGroupService,
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISettingProfileService settingProfileService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        ICustomerSettingHelper helperService)
        : base(dynamicRepository, appSettings, settingProfileService, cacheRefService, messageService, null, helperService)
    {
        this.customerService = customerService;
        this.siteDBGroupService = siteDBGroupService;
    }

    private readonly IDictionary<string, string> CommissionNameMappings = new Dictionary<string, string>()
    {
        {"1_1","a"},
        {"1_3","b"},
        {"1_7","c"},
        {"1_8","d"}
    };

    private decimal ptStep = 1;

    private bool isAllowUncheckAll = false;

    private bool IsAllowUncheckAll(int roleId, string site, string siteLabel, AppSettings appSettings)
    {
        var isY7 = appSettings.IsSpecialSites(siteLabel);
        var IsAllowUncheckStakeLevelOnMemberSite = appSettings.IsAllowUncheckStakeLevelOnMember(site);
        if (roleId != CustomerLevel.Member && !isY7)
            return true;

        if (roleId == CustomerLevel.Member && IsAllowUncheckStakeLevelOnMemberSite)
            return true;

        return false;
    }

    public async Task<CustomerSettingModel> QuerySuper(QuerySuperRequest request)
    {
        ptStep = appSettings.IsSupportPT3Decimal(request.SiteLabel) ? 0.5M : 1M;
        var generalInfo = new GeneralInformation();
        var settingProfile = await settingProfileService.GetSettingProfiles(request.SystemId, request.Site, request.Currency, ProfileMode.AddNew);
        var customerSetting = new CustomerSettingModel
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(
                settingProfile, 
                request, 
                appSettings.Belonging777Group(request.SiteLabel), 
                generalInfo)
        };

        customerSetting.GeneralInfo.UplineRoleId = 0;
        return customerSetting;
    }

    public async Task<CustomerSettingModel> Query(QueryRequest request)
    {
        var generalInfo = await GetGeneralInfo(request);
        ptStep = appSettings.IsSupportPT3Decimal(IsDirectMaster(request) ? request.SiteLabel : generalInfo.SiteLabel) ? 0.5M : 1M;
        isAllowUncheckAll = IsAllowUncheckAll(request.RoleId, generalInfo.Site, IsDirectMaster(request) ? request.SiteLabel : generalInfo.SiteLabel, appSettings);
        var settingProfile = await settingProfileService.GetSettingProfiles(request.SystemId, generalInfo.Site, generalInfo.Currency, request.IsGetUpline ? ProfileMode.Edit : ProfileMode.AddNew);
        if (request.RoleId == CustomerLevel.Agent)
        {
            generalInfo.Commissions = await GetAgentSportsBookCommissions(request.CustId);
        }

        var customerSetting = new CustomerSettingModel
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(settingProfile, request, appSettings.Belonging777Group(generalInfo.SiteLabel), generalInfo)
        };

        customerSetting.GeneralInfo.UplineRoleId = request.RoleId;
        return customerSetting;
    }

    private async Task<GeneralInformation> GetGeneralInfo(QueryRequest request)
    {
        var generalInfo = await dynamicRepository.GetAsync<GeneralInformation>(request.IsGetUpline
            ? new GetGeneralInfoForEditCriteria(request.CustId, request.SystemId)
            : new GetCustomerCriteria(request.CustId, appSettings.SettingStatus, request.RoleId));

        if (generalInfo != null && generalInfo.RoleId == CustomerLevel.Member)
        {
            var siteDBName = await siteDBGroupService.GetSiteDBName(generalInfo.Site);
            var memberBalance = await customerService.GetMemberBalance(request.CustId, siteDBName);
            generalInfo.MaxCreditMin = Math.Max(0, memberBalance.CustomerOutstanding - memberBalance.CustomerCash + memberBalance.PendingTransfer3rd);
        }

        return generalInfo;
    }

    private async Task<IEnumerable<SportsBookCommission>> GetAgentSportsBookCommissions(int agentCustId)
    {
        var commissions = await dynamicRepository.FetchAsync<SportsBookCommission>(new GetAgentSportsBookCommissionsCriteria(agentCustId));
        foreach (var commission in commissions)
        {
            commission.Name = CommissionNameMappings[$"{commission.SportType}_{commission.BetType}"];
        }
        return commissions;
    }

    private IEnumerable<ProductGroup> GetProductGroups(
        IEnumerable<SettingProfile> settingProfile, 
        BaseQueryRequest request, 
        bool isSiteLabel777,
        GeneralInformation generalInfo)
    {
        return settingProfile.Where(p => p.DisplayMode != 3 && (!request.ProductGroupId.HasValue || request.ProductGroupId.Value == 0 || (request.ProductGroupId > 0 && p.GroupId == request.ProductGroupId))).GroupBy(p => p.GroupId)
        .Select((g) => new ProductGroup
        {
            Id = g.Key,
            Name = cacheRefService.GetProductGroupName(g.Key, request.Language),
            GroupTypes = ((typeof(QueryRequest) == request.GetType()) ?
            (((QueryRequest)request).IsGetUpline ? GetGroupTypesForEdit(g, (QueryRequest)request, isSiteLabel777, generalInfo) : GetGroupTypes(g, (QueryRequest)request, isSiteLabel777)) :
            GetGroupTypesForSuper(g, (QuerySuperRequest)request, isSiteLabel777)).Where(gt => gt != null)
        }).Where(p => p.GroupTypes != null && p.GroupTypes.Any(gt => gt != null && gt.GroupItems.Any()));
    }

    private IEnumerable<GroupType> GetGroupTypesForSuper(IGrouping<int, SettingProfile> g, QuerySuperRequest request, bool isSiteLabel777)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevel(t.Key.GroupType, (int)CustomerLevel.Super) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QuerySuperForAddNew(request.Site, request.Currency, g.Key, t.Key.GroupType);
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    IsAllowUncheckAll = isAllowUncheckAll,
                    Name = Resources.Resources.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
                    DisplayType = t.Key.DisplayType,
                    GroupItems = GetGroupItems(
                        g.Key,
                        t.Key.GroupType,
                        t.Key.DisplayType,
                        0,
                        false,
                        request.Language,
                        groupItems,
                        t,
                        isSiteLabel777)
                };

                return groupType;
            }

            return null;
        }).ToEnumerable().ToList(); // Must use ToList to prevent call SP 2 times because of Any method in the next action.
    }

    private IEnumerable<GroupType> GetGroupTypes(IGrouping<int, SettingProfile> g, QueryRequest request, bool isSiteLabel777)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevel(t.Key.GroupType, (byte)(request.RoleId - 1)) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForAddNew(request.CustId, request.RoleId, g.Key, t.Key.GroupType);
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    IsAllowUncheckAll = isAllowUncheckAll,
                    Name = Resources.Resources.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
                    DisplayType = t.Key.DisplayType,
                    GroupItems = GetGroupItems(
                        g.Key,
                        t.Key.GroupType,
                        t.Key.DisplayType,
                        request.RoleId - 1,
                        request.IsGetUpline,
                        request.Language,
                        groupItems,
                        t,
                        isSiteLabel777)
                };

                return groupType;
            }

            return null;
        }).ToEnumerable().ToList(); // Must use ToList to prevent call SP 2 times because of Any method in the next action.
    }

    private IEnumerable<GroupType> GetGroupTypesForEdit(
        IGrouping<int, SettingProfile> g, 
        QueryRequest request, 
        bool isSiteLabel777,
        GeneralInformation generalInfo)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevelAndUplineLevel(t.Key.GroupType, request.RoleId, request.UplineRoleId.Value) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForEdit(
                    request.CustId, 
                    request.RoleId, 
                    request.UplineRoleId, 
                    g.Key, 
                    t.Key.GroupType, 
                    request.IsMultipleMode,
                    generalInfo.Currency,
                    generalInfo.Site,
                    request.SystemId,
                    request.GroupName,
                    request.IsCopy);
              
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    IsAllowUncheckAll = isAllowUncheckAll,
                    Name = Resources.Resources.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
                    DisplayType = t.Key.DisplayType,
                    IsReadOnly = IsReadOnlyGroupType(request, t.Key.GroupType),
                    GroupItems = GetGroupItems(
                        g.Key,
                        t.Key.GroupType,
                        t.Key.DisplayType,
                        request.RoleId,
                        request.IsGetUpline,
                        request.Language,
                        groupItems,
                        t,
                        isSiteLabel777)
                };

                return groupType;
            }

            return null;
        }).ToEnumerable().ToList(); // Must use ToList to prevent call SP 2 times because of Any method in the next action.
    }

    private bool IsReadOnlyGroupType(QueryRequest request, int groupTypeId)
    {
        if ((request.SystemId == Systems.EXT || request.SystemId == Systems.EXTY7) && request.RoleId == CustomerLevel.Super)
        {
            return true;
        }

        return IsCrossline(request) && !helperService.IsPTGroupType(groupTypeId);
    }

    private static bool IsCrossline(QueryRequest request)
    {
        return request.UplineRoleId - request.RoleId > 1;
    }

    private IEnumerable<GroupItemDefault> GetGroupItems(
        int groupId,
        int groupTypeId,
        int displayType,
        int roleId,
        bool isGetUpline,
        string language,
        IEnumerable<GroupItemDefault> groupItems,
        IGrouping<object, SettingProfile> t,
        bool isSiteLabel777)
    {
        return groupItems.Join(
        t.GroupBy(sb => new { sb.SubGroupId, sb.SportType, sb.BetType }),
        groupItems => new { SportType = groupItems.SportTypeId, BetType = groupItems.BetTypeId },
        groupItemsProfile => new { groupItemsProfile.Key.SportType, groupItemsProfile.Key.BetType },
        (groupItem, groupItemsProfile) =>
        {
            if (isGetUpline)
            {
                groupItem.MapUpline();
            }

            groupItem.ProductName = cacheRefService.GetProductName(groupItem.ProductId, language);
            groupItem.SubGroupId = groupItemsProfile.Key.SubGroupId;
            groupItem.SubGroupName = cacheRefService.GetSubGroupName(groupItemsProfile.Key.SubGroupId, language);
            groupItem.SportTypeName = cacheRefService.GetSportTypeName(groupItem.SportTypeId, language);
            groupItem.BetTypeName = cacheRefService.GetBetTypeName(groupItem.BetTypeId, language);
            groupItem.DisplayName = cacheRefService.GetDisplayNameByDisplayType(
                groupItem.SportTypeId,
                groupItem.BetTypeId,
                groupId,
                groupTypeId,
                displayType,
                roleId,
                language,
                isSiteLabel777);

            if (helperService.IsPTGroupType(groupTypeId))
            {
                ((PTItem)groupItem).Step = ptStep;
            }

            return groupItem;
        });
    }

    private static bool IsDirectMaster(QueryRequest request)
    => request.CustId <= 100 && !string.IsNullOrWhiteSpace(request.SiteLabel);
}