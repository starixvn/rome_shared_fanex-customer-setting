using System.Linq.Expressions;
using EasyNetQ.ConnectionString;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings;
using Fanex.Customer.API.Modules.CreateCustomer.Entities;
using Fanex.Customer.API.Modules.CreateCustomer.Helpers;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Resources;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using static Fanex.Customer.API.Modules.CreateCustomer.SharedServices.SharedService;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface IQueryCustomerService
{
    Task<CustomerSetting> QuerySuper(QuerySuperRequest request, bool isSportsbookOnly);

    Task<CustomerSetting> Query(QueryRequest request, bool isSportsbookOnly);
}

public class QueryCustomerService : BaseCustomerService, IQueryCustomerService
{
    public QueryCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISharedService sharedService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        ICustomerSettingHelper helperService)
        : base(dynamicRepository, appSettings, sharedService, cacheRefService, messageService, null, helperService)
    {
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

        if  (roleId == CustomerLevel.Member && IsAllowUncheckStakeLevelOnMemberSite)
            return true;

        return false;
    }

    public async Task<CustomerSetting> QuerySuper(QuerySuperRequest request, bool isSportsbookOnly)
    {
        ptStep = appSettings.IsSupportPT3Decimal(request.SiteLabel) ? 0.5M : 1M;
        var generalInfo = new GeneralInformation();
        var settingProfile = await sharedService.GetSettingProfiles(request.SystemId, request.Site, request.Currency, ProfileMode.AddNew);
        var customerSetting = new CustomerSetting
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(settingProfile, request, appSettings.Belonging777Group(request.SiteLabel))
        };

        customerSetting.GeneralInfo.UplineRoleId = 0;
        return customerSetting;
    }

    public async Task<CustomerSetting> Query(QueryRequest request, bool isSportsbookOnly)
    {
        var generalInfo = await GetGeneralInfo(request);
        ptStep = appSettings.IsSupportPT3Decimal(IsDirectMaster(request) ? request.SiteLabel : generalInfo.SiteLabel) ? 0.5M : 1M;
        isAllowUncheckAll = IsAllowUncheckAll(request.RoleId, generalInfo.Site, IsDirectMaster(request) ? request.SiteLabel : generalInfo.SiteLabel, appSettings);
        var settingProfile = await sharedService.GetSettingProfiles(request.SystemId, generalInfo.Site, generalInfo.Currency, request.IsGetUpline ? ProfileMode.Edit : ProfileMode.AddNew);
        if (request.RoleId == (byte)CustomerLevel.Agent)
        {
            generalInfo.Commissions = await GetAgentSportsBookCommissions(request.CustId);
        }

        var customerSetting = new CustomerSetting
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(settingProfile, request, appSettings.Belonging777Group(generalInfo.SiteLabel))
        };

        customerSetting.GeneralInfo.UplineRoleId = request.RoleId;
        return customerSetting;
    }

    private async Task<GeneralInformation> GetGeneralInfo(QueryRequest request)
    {
        return await dynamicRepository.GetAsync<GeneralInformation>(request.IsGetUpline ?
        new GetGeneralInfoForEditCriteria(request.CustId, request.SystemId) :
        new GetCustomerCriteria(request.CustId, appSettings.SettingStatus, request.RoleId));
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

    private IEnumerable<ProductGroup> GetProductGroups(IEnumerable<SettingProfile> settingProfile, BaseQueryRequest request, bool isSiteLabel777)
    {
        return settingProfile.Where(p => p.DisplayMode != 3 && (!request.ProductGroupId.HasValue || request.ProductGroupId.Value == 0 || (request.ProductGroupId > 0 && p.GroupId == request.ProductGroupId))).GroupBy(p => p.GroupId)
        .Select((g) => new ProductGroup
        {
            Id = g.Key,
            Name = cacheRefService.GetProductGroupName(g.Key, request.Language),
            GroupTypes = ((typeof(QueryRequest) == request.GetType()) ?
            (((QueryRequest)request).IsGetUpline ? GetGroupTypesForEdit(g, (QueryRequest)request, isSiteLabel777) : GetGroupTypes(g, (QueryRequest)request, isSiteLabel777)) :
            GetGroupTypesForSuper(g, (QuerySuperRequest)request, isSiteLabel777)).Where(gt => gt != null)
        }).Where(p => p.GroupTypes != null && p.GroupTypes.Any(gt => gt != null && gt.GroupItems.Any()));
    }

    private IEnumerable<GroupType> GetGroupTypesForSuper(IGrouping<int, SettingProfile> g, QuerySuperRequest request, bool isSiteLabel777)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevel(t.Key.GroupType, (int)CustomerLevel.Super) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QuerySuperForAddNew(request.Site, request.Currency, g.Key, t.Key.GroupType);
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    IsAllowUncheckAll = isAllowUncheckAll,
                    Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
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
            var groupTypeWithRoleId = IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevel(t.Key.GroupType, (byte)(request.RoleId - 1)) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForAddNew(request.CustId, request.RoleId, g.Key, t.Key.GroupType);
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    IsAllowUncheckAll = isAllowUncheckAll,
                    Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
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

    private IEnumerable<GroupType> GetGroupTypesForEdit(IGrouping<int, SettingProfile> g, QueryRequest request, bool isSiteLabel777)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevelAndUplineLevel(t.Key.GroupType, request.RoleId, request.UplineRoleId.Value) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForEdit(request.CustId, request.RoleId, request.UplineRoleId, g.Key, t.Key.GroupType);
                var groupType = new GroupType
                {
                    Id = t.Key.GroupType,
                    IsAllowUncheckAll = isAllowUncheckAll,
                    Name = Labels.ResourceManager.GetString($"GroupType{t.Key.GroupType}", new System.Globalization.CultureInfo(request.Language)),
                    DisplayType = t.Key.DisplayType,
                    IsReadOnly = request.UplineRoleId - request.RoleId > 1 && !IsPTGroupType(t.Key.GroupType),
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
            groupItem.SportTypeName = IsSabaCasino(groupItem.SportTypeId, groupItem.BetTypeId) ? cacheRefService.GetProductName(SabaCasinoProductId, language) : cacheRefService.GetSportTypeName(groupItem.SportTypeId, language);
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

            if (IsPTGroupType(groupTypeId))
            {
                ((PTItem)groupItem).Step = ptStep;
            }

            return groupItem;
        });
    }

    private bool IsDirectMaster(QueryRequest request)
    => request.CustId <= 100 && !string.IsNullOrWhiteSpace(request.SiteLabel);
}