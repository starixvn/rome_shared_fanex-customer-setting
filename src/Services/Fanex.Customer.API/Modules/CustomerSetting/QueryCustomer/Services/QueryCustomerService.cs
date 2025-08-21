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
    private const decimal DefaultPtStep = 1;
    private const decimal DecimalPtStep = 0.5M;
    private const int DirectMasterCustIdThreshold = 100;
    private const int DefaultCrosslineThreshold = 1;

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

    private static readonly IReadOnlyDictionary<string, string> CommissionNameMappings = new Dictionary<string, string>
    {
        {"1_1","a"},
        {"1_3","b"},
        {"1_7","c"},
        {"1_8","d"}
    };

    private decimal ptStep = DefaultPtStep;

    private bool isAllowUncheckAll;

    private bool IsAllowUncheckAll(int roleId, string site, string siteLabel, bool? isDirectMember, AppSettings appSettings)
    {
        var isY7 = appSettings.IsSpecialSites(siteLabel);
        var IsAllowUncheckStakeLevelOnMemberSite = appSettings.IsAllowUncheckStakeLevelOnMember(site);

        if (roleId != CustomerLevel.Member && !isDirectMember.GetValueOrDefault() && !isY7)
        {
            return true;
        }

        if ((roleId == CustomerLevel.Member || isDirectMember.GetValueOrDefault()) && IsAllowUncheckStakeLevelOnMemberSite)
        {
            return true;
        }

        return false;
    }

    private decimal GetPtStep(string siteLabel)
        => appSettings.IsSupportPT3Decimal(siteLabel) ? DecimalPtStep : DefaultPtStep;

    private bool DetermineDirectMemberStatus(QueryRequest request)
        => request.IsGetUpline
            ? helperService.IsEditDirectMember(request.RoleId, request.UplineRoleId.GetValueOrDefault(), request.AgentId)
            : helperService.IsCreateNewDirectMember(request.RoleId, request.UplineRoleId.GetValueOrDefault(), request.AgentId);

    private void SetupQueryContext(QueryRequest request, GeneralInformation generalInfo, bool isDirectMember)
    {
        var siteLabel = IsDirectMaster(request) ? request.SiteLabel : generalInfo.SiteLabel;
        ptStep = GetPtStep(siteLabel);
        isAllowUncheckAll = IsAllowUncheckAll(request.RoleId, generalInfo.Site, siteLabel, isDirectMember, appSettings);
    }

    private async Task<IEnumerable<SettingProfile>> GetSettingProfile(QueryRequest request, GeneralInformation generalInfo, bool isDirectMember)
    {
        var profileMode = DetermineProfileMode(request, isDirectMember);
        return await settingProfileService.GetSettingProfiles(request.SystemId, generalInfo.Site, generalInfo.Currency, profileMode);
    }

    private static ProfileMode DetermineProfileMode(QueryRequest request, bool isDirectMember)
    {
        if (isDirectMember) 
        {
            return ProfileMode.DirectMember;
        }
        
        return request.IsGetUpline ? ProfileMode.Edit : ProfileMode.AddNew;
    }

    private static bool ShouldLoadCommissions(QueryRequest request, bool isDirectMember)
    {
        var isUplineEdit = request.IsGetUpline && request.RoleId == CustomerLevel.Member;
        var isDownlineCreate = !request.IsGetUpline && (request.RoleId == CustomerLevel.Agent || isDirectMember);
        return isUplineEdit || isDownlineCreate;
    }

    public async Task<CustomerSettingModel> QuerySuper(QuerySuperRequest request)
    {
        ptStep = GetPtStep(request.SiteLabel);
        var generalInfo = new GeneralInformation();
        var settingProfile = await settingProfileService.GetSettingProfiles(
            request.SystemId, 
            request.Site, 
            request.Currency, 
            ProfileMode.AddNew);
            
        var customerSetting = new CustomerSettingModel
        {
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(
                settingProfile,
                request,
                appSettings.Belonging777Group(request.SiteLabel),
                generalInfo,
                false,
                generalInfo.UplineRoleId)
        };

        customerSetting.GeneralInfo.UplineRoleId = 0;
        return customerSetting;
    }

    public async Task<CustomerSettingModel> Query(QueryRequest request)
    {
        var isDirectMember = DetermineDirectMemberStatus(request);
        var generalInfoDto = await GetGeneralInfo(request, isDirectMember);
        var generalInfo = new GeneralInformation(generalInfoDto);
        var uGroup = generalInfoDto.Group;
        
        SetupQueryContext(request, generalInfo, isDirectMember);
        
        var settingProfile = await GetSettingProfile(request, generalInfo, isDirectMember);
        
        if (ShouldLoadCommissions(request, isDirectMember))
        {
            generalInfo.Commissions = await GetAgentSportsBookCommissions(request.CustId);
        }

        var role = request.IsGetUpline ? request.UplineRoleId.GetValueOrDefault() : request.RoleId;
        var customerSetting = new CustomerSettingModel
        {
            Group = uGroup,
            GeneralInfo = generalInfo,
            ProductGroups = GetProductGroups(settingProfile, request, appSettings.Belonging777Group(generalInfo.SiteLabel), generalInfo, isDirectMember, role)
        };
        
        await SetProductStatusTooltip(customerSetting, request.Language, request.CustId, appSettings.SettingStatus);
        customerSetting.GeneralInfo.UplineRoleId = request.RoleId;
        return customerSetting;
    }

    private async Task<GeneralInformationDTO> GetGeneralInfo(QueryRequest request, bool isDirectMember)
    {
        var criteria = CreateGetGeneralInfoCriteria(request, isDirectMember);
        var generalInfo = await dynamicRepository.GetAsync<GeneralInformationDTO>((ICriteria)criteria);

        if (generalInfo?.RoleId == CustomerLevel.Member)
        {
            await UpdateMemberCreditInfo(generalInfo, request.CustId);
        }

        return generalInfo;
    }

    private object CreateGetGeneralInfoCriteria(QueryRequest request, bool isDirectMember)
    {
        if (request.IsGetUpline)
        {
            return new GetGeneralInfoForEditCriteria(request.CustId, request.SystemId);
        }

        if (request.RoleId == CustomerLevel.Agent)
        {
            return new GetAgentCriteria(request.CustId, appSettings.SettingStatus, request.RoleId);
        }

        return new GetSuperMasterCriteria(request.CustId, appSettings.SettingStatus, isDirectMember, request.RoleId);
    }

    private async Task UpdateMemberCreditInfo(GeneralInformationDTO generalInfo, int custId)
    {
        var siteDBName = await siteDBGroupService.GetSiteDBName(generalInfo.Site);
        var memberBalance = await customerService.GetMemberBalance(custId, siteDBName);
        generalInfo.MaxCreditMin = Math.Max(0, memberBalance.CustomerOutstanding - memberBalance.CustomerCash + memberBalance.PendingTransfer3rd);
    }

    private int GetGroupTypeWithRoleId(int groupType, QueryRequest request)
    {
        if (!helperService.IsGroupTypeByLevel(groupType))
        {
            return groupType;
        }

        var customerLevel = request.AgentId < 0 ? CustomerLevel.Member : (byte)(request.RoleId - 1);
        var uplineLevel = request.IsGetUpline ? request.UplineRoleId.GetValueOrDefault() : request.RoleId;
        
        return helperService.GetGroupTypeByCustomerLevelAndUplineLevel(
            groupType,
            customerLevel,
            uplineLevel,
            request.AgentId);
    }

    private int GetGroupTypeForEdit(int groupType, QueryRequest request)
    {
        if (!helperService.IsGroupTypeByLevel(groupType))
        {
            return groupType;
        }

        return helperService.GetGroupTypeByCustomerLevelAndUplineLevel(
            groupType, 
            request.RoleId, 
            request.UplineRoleId.Value, 
            request.AgentId);
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
        GeneralInformation generalInfo,
        bool isDirectMember,
        byte uplineRoleId)
    {
        var filteredProfiles = FilterSettingProfiles(settingProfile, request, isDirectMember, uplineRoleId);
        
        return filteredProfiles.GroupBy(p => p.GroupId)
            .Select(g => CreateProductGroup(g, request, isSiteLabel777, generalInfo, isDirectMember))
            .Where(p => HasValidGroupTypes(p));
    }

    private IEnumerable<SettingProfile> FilterSettingProfiles(
        IEnumerable<SettingProfile> settingProfile,
        BaseQueryRequest request,
        bool isDirectMember,
        byte uplineRoleId)
    {
        return settingProfile.Where(p => 
            !IsHideGroupType(p.DisplayMode) &&
            IsValidProductGroup(request, p) &&
            SupportPotentialWinLimitByRole(p.GroupType, isDirectMember, uplineRoleId));
    }

    private static bool IsValidProductGroup(BaseQueryRequest request, SettingProfile profile)
    {
        if (!request.ProductGroupId.HasValue) 
        {
            return true;
        }
        
        if (request.ProductGroupId.Value == 0) 
        {
            return true;
        }
        
        return request.ProductGroupId > 0 && profile.GroupId == request.ProductGroupId;
    }

    private ProductGroup CreateProductGroup(
        IGrouping<int, SettingProfile> grouping,
        BaseQueryRequest request,
        bool isSiteLabel777,
        GeneralInformation generalInfo,
        bool isDirectMember)
    {
        var groupTypes = GetGroupTypesForRequest(grouping, request, isSiteLabel777, generalInfo, isDirectMember);
        
        return new ProductGroup
        {
            Id = grouping.Key,
            Name = cacheRefService.GetProductGroupName(grouping.Key, request.Language),
            GroupTypes = groupTypes.Where(gt => gt != null)
        };
    }

    private IEnumerable<GroupType> GetGroupTypesForRequest(
        IGrouping<int, SettingProfile> grouping,
        BaseQueryRequest request,
        bool isSiteLabel777,
        GeneralInformation generalInfo,
        bool isDirectMember)
    {
        return request switch
        {
            QueryRequest queryRequest when queryRequest.IsGetUpline => 
                GetGroupTypesForEdit(grouping, queryRequest, isSiteLabel777, generalInfo, isDirectMember),
            QueryRequest queryRequest => 
                GetGroupTypes(grouping, queryRequest, isSiteLabel777, isDirectMember),
            QuerySuperRequest querySuperRequest => 
                GetGroupTypesForSuper(grouping, querySuperRequest, isSiteLabel777),
            _ => Enumerable.Empty<GroupType>()
        };
    }

    private static bool HasValidGroupTypes(ProductGroup productGroup)
        => productGroup.GroupTypes != null && 
           productGroup.GroupTypes.Any(gt => gt != null && gt.GroupItems.Any());

    private bool SupportPotentialWinLimitByRole(int groupTypeId, bool isDirectMember, byte uplineRoleId)
    {
        if (groupTypeId == GroupTypes.PotentialWinLimit)
        {
            return isDirectMember || uplineRoleId == CustomerLevel.Agent;
        }

        return true;
    }

    private IEnumerable<GroupType> GetGroupTypesForSuper(IGrouping<int, SettingProfile> g, QuerySuperRequest request, bool isSiteLabel777)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType })
            .ToAsyncEnumerable()
            .SelectAwait(async t =>
            {
                var groupTypeWithRoleId = helperService.IsGroupTypeByLevel(t.Key.GroupType)
                    ? helperService.GetGroupTypeByCustomerLevel(t.Key.GroupType, (int)CustomerLevel.Super)
                    : t.Key.GroupType;

                if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
                {
                    var groupItems = await value.QuerySuperForAddNew(
                        request.Site,
                        request.Currency,
                        g.Key,
                        t.Key.GroupType);

                    var groupType = CreateGroupType(
                        t.Key.GroupType,
                        t.Key.DisplayType,
                        isAllowUncheckAll,
                        request.Language);
                    
                    groupType.GroupItems = GetGroupItems(
                        g.Key,
                        t.Key.GroupType,
                        t.Key.DisplayType,
                        0,
                        false,
                        null,
                        request.Language,
                        groupItems,
                        t,
                        isSiteLabel777,
                        ptStep);

                    return groupType;
                }

                return null;
            })
            .ToEnumerable()
            .ToList(); // Must use ToList to prevent call SP 2 times because of Any method in the next action.
    }

    private IEnumerable<GroupType> GetGroupTypes(
        IGrouping<int, SettingProfile> g,
        QueryRequest request,
        bool isSiteLabel777,
        bool isDirectMember)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = GetGroupTypeWithRoleId(t.Key.GroupType, request);
            if (groupItemMapping.TryGetValue(groupTypeWithRoleId, out dynamic value))
            {
                var groupItems = await value.QueryForAddNew(request.CustId, request.RoleId, g.Key, t.Key.GroupType, isDirectMember);
                var groupType = CreateGroupType(
                    t.Key.GroupType,
                    t.Key.DisplayType,
                    isAllowUncheckAll,
                    request.Language);
                
                groupType.GroupItems = GetGroupItems(
                    g.Key,
                    t.Key.GroupType,
                    t.Key.DisplayType,
                    request.RoleId - 1,
                    request.IsGetUpline,
                    isDirectMember,
                    request.Language,
                    groupItems,
                    t,
                    isSiteLabel777,
                    ptStep);

                return groupType;
            }

            return null;
        }).ToEnumerable().ToList(); // Must use ToList to prevent call SP 2 times because of Any method in the next action.
    }

    private IEnumerable<GroupType> GetGroupTypesForEdit(
        IGrouping<int, SettingProfile> g,
        QueryRequest request,
        bool isSiteLabel777,
        GeneralInformation generalInfo,
        bool isDirectMember)
    {
        return g.GroupBy(t => new { t.GroupType, t.DisplayType }).ToAsyncEnumerable()
        .SelectAwait(async t =>
        {
            var groupTypeWithRoleId = GetGroupTypeForEdit(t.Key.GroupType, request);
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
                    request.IsCopy,
                    request.AgentId);

                var groupType = CreateGroupType(
                    t.Key.GroupType,
                    t.Key.DisplayType,
                    isAllowUncheckAll,
                    request.Language,
                    IsReadOnlyGroupType(request, t.Key.GroupType));
                
                groupType.GroupItems = GetGroupItems(
                    g.Key,
                    t.Key.GroupType,
                    t.Key.DisplayType,
                    request.RoleId,
                    request.IsGetUpline,
                    isDirectMember,
                    request.Language,
                    groupItems,
                    t,
                    isSiteLabel777,
                    ptStep);

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

    private bool IsCrossline(QueryRequest request)
    {
        var isEditDirectMember = helperService.IsEditDirectMember(request.RoleId, request.UplineRoleId.GetValueOrDefault(), request.AgentId);
        
        if (isEditDirectMember)
        {
            return helperService.IsEditCrosslineDirectMember(request.RoleId, request.UplineRoleId.GetValueOrDefault(), request.AgentId);
        }

        return request.UplineRoleId - request.RoleId > DefaultCrosslineThreshold;
    }

    private static bool IsDirectMaster(QueryRequest request)
        => request.CustId <= DirectMasterCustIdThreshold && !string.IsNullOrWhiteSpace(request.SiteLabel);
}