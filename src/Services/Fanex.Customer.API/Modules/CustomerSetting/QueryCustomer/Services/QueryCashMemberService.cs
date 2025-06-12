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

public interface IQueryCashCustomerService
{
    Task<CustomerSettingModel> Query(QueryCashCustomerRequest request);
}

public class QueryCashCustomerService : BaseQueryCustomerService, IQueryCashCustomerService
{
    private readonly ICustomerService customerService;

    private readonly ISiteDBGroupService siteDBGroupService;

    public QueryCashCustomerService(
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

    public async Task<CustomerSettingModel> Query(QueryCashCustomerRequest request)
    {
        var generalInfoDto = await GetGeneralInfo(request);
        var generalInfo = new GeneralInformation(generalInfoDto);
        ptStep = appSettings.IsSupportPT3Decimal(generalInfo.SiteLabel) ? 0.5M : 1M;
        isAllowUncheckAll = IsAllowUncheckAll(request.IsGetUpline ? request.RoleId : request.RoleId - 1, generalInfo.Site, generalInfo.SiteLabel, appSettings);
        var settingProfile = await settingProfileService.GetSettingProfiles(
            request.SystemId, 
            generalInfo.Site, 
            generalInfo.Currency, 
            request.IsGetUpline ? ProfileMode.Edit : ProfileMode.AddNew, 
            ServiceType.Cash);

        var customerSetting = new CustomerSettingModel
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(settingProfile, request, appSettings.Belonging777Group(generalInfo.SiteLabel), generalInfo)
        };

        await SetProductStatusTooltip(customerSetting, request.Language, request.CustId, appSettings.SettingStatus);
        customerSetting.GeneralInfo.UplineRoleId = request.RoleId;
        return customerSetting;
    }

    private async Task<GeneralInformationDTO> GetGeneralInfo(QueryCashCustomerRequest request)
        => await dynamicRepository.GetAsync<GeneralInformationDTO>(request.IsGetUpline
            ? new GetGeneralInfoForEditCriteria(request.CustId, request.SystemId)
            : new GetAgentCriteria(request.CustId, appSettings.SettingStatus, CustomerLevel.Agent));

    private IEnumerable<ProductGroup> GetProductGroups(
        IEnumerable<SettingProfile> settingProfile,
        BaseQueryRequest request,
        bool isSiteLabel777,
        GeneralInformation generalInfo)
    {
        var cashRequest = (QueryCashCustomerRequest)request;
        var productGroupId = request.ProductGroupId.GetValueOrDefault(0);
        var isGetUpline = cashRequest.IsGetUpline;
        return settingProfile.Where(p => 
            !IsHideGroupType(p.DisplayMode) && 
            (productGroupId == 0 || p.GroupId == request.ProductGroupId))
            .GroupBy(p => p.GroupId)
        .Select((g) => new ProductGroup
        {
            Id = g.Key,
            Name = cacheRefService.GetProductGroupName(g.Key, request.Language),
            GroupTypes = isGetUpline ? GetGroupTypesForEdit(g, cashRequest, isSiteLabel777, generalInfo, false) : GetGroupTypes(g, cashRequest, isSiteLabel777, false)
        }).Where(p => p.GroupTypes != null && p.GroupTypes.Any(gt => gt != null && gt.GroupItems.Any()));
    }

    private IEnumerable<GroupType> GetGroupTypes(
        IGrouping<int, SettingProfile> g,
        QueryCashCustomerRequest request,
        bool isSiteLabel777,
        bool isDirectMember)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType) ?
                helperService.GetGroupTypeByCustomerLevel(
                    t.Key.GroupType,
                    (byte)(request.RoleId - 1))
                : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForAddNew(request.CustId, request.RoleId, g.Key, t.Key.GroupType, isDirectMember);
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
                        isDirectMember,
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
        QueryCashCustomerRequest request,
        bool isSiteLabel777,
        GeneralInformation generalInfo,
        bool isDirectMember)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType) ? helperService.GetGroupTypeByCustomerLevelAndUplineLevel(t.Key.GroupType, request.RoleId, request.UplineRoleId.Value, 0) : t.Key.GroupType;
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForEdit(
                    request.CustId,
                    request.RoleId,
                    request.UplineRoleId,
                    g.Key,
                    t.Key.GroupType,
                    false,
                    generalInfo.Currency,
                    generalInfo.Site,
                    request.SystemId,
                    string.Empty,
                    false,
                    0);

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
                        isDirectMember,
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

    private bool IsReadOnlyGroupType(QueryCashCustomerRequest request, int groupTypeId)
    {
        if ((request.SystemId == Systems.EXT || request.SystemId == Systems.EXTY7) && request.RoleId == CustomerLevel.Super)
        {
            return true;
        }

        return IsCrossline(request) && !helperService.IsPTGroupType(groupTypeId);
    }

    private bool IsCrossline(QueryCashCustomerRequest request)
    {
        return request.UplineRoleId - request.RoleId > 1;
    }

    private IEnumerable<GroupItemDefault> GetGroupItems(
        int groupId,
        int groupTypeId,
        int displayType,
        int roleId,
        bool isGetUpline,
        bool? isDirectMember,
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
                isDirectMember,
                language,
                isSiteLabel777);

            if (helperService.IsPTGroupType(groupTypeId))
            {
                ((PTItem)groupItem).Step = ptStep;
            }

            return groupItem;
        });
    }
}