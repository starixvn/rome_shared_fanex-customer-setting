using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType158;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType150;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType151;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType152;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType153;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType154;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType155;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType156;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType158;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType160;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.QueryCustomer.Services;

public interface IQueryGroupTypeBaseService<T>
{
    Task<IEnumerable<T>> QueryForAddNew(int uplineCustId, int uplineRoleId, int productGroupId, int groupType, bool? isDirectMember);

    Task<IEnumerable<T>> QuerySuperForAddNew(string site, int currency, int groupId, int groupType);

    Task<IEnumerable<T>> QueryForEdit(
        int custId, 
        byte roleId, 
        byte? uplineRoleId,
        int groupId, 
        int groupType, 
        bool isMultipleMode,
        int currency,
        string site,
        byte systemId,
        string userGroup,
        bool isCopy,
        bool? isDirectMember);
}

public class QueryGroupTypeBaseService<T> : IQueryGroupTypeBaseService<T>
{
    protected readonly Dictionary<int, dynamic> criteriaMapping;

    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    public QueryGroupTypeBaseService(IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings, ICustomerSettingHelper helperService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.helperService = helperService;
    }

    public async Task<IEnumerable<T>> QueryForAddNew(int uplineCustId, int uplineRoleId, int productGroupId, int groupType, bool? isDirectMember)
        => await dynamicRepository.FetchAsync<T>(new GetCustomerCriteria(
            uplineCustId,
            appSettings.SettingStatus,
            isDirectMember,
            uplineRoleId,
            productGroupId,
            groupType));

    public async Task<IEnumerable<T>> QuerySuperForAddNew(string site, int currency, int groupId, int groupType)
        => await dynamicRepository.FetchAsync<T>(
            new GetSettingBySiteCurrencyCriteria(
                site,
                currency,
                groupId,
                groupType,
                appSettings.SettingStatus));

    public async Task<IEnumerable<T>> QueryForEdit(
        int custId, 
        byte roleId, 
        byte? uplineRoleId, 
        int groupId, 
        int groupType, 
        bool isMultipleMode,
        int currency,
        string site,
        byte systemId,
        string userGroup,
        bool isCopy,
        bool? isDirectMember)
    {
        var criteria = GetQueryForEditCriteria(
            custId, 
            roleId, 
            uplineRoleId, 
            groupId, 
            groupType, 
            isMultipleMode,
            currency,
            site,
            systemId,
            userGroup,
            isCopy,
            isDirectMember);

        if (criteria == null)
        {
            return Enumerable.Empty<T>();
        }

        return await dynamicRepository.FetchAsync<T>(criteria);
    }

    private ICriteria GetQueryForEditCriteria(
        int custId, 
        byte roleId, 
        byte? uplineRoleId, 
        int groupId, 
        int groupType, 
        bool isMultipleMode,
        int currency,
        string site,
        byte systemId,
        string userGroup,
        bool isCopy,
        bool? isDirectMember)
    {
        if(isMultipleMode 
            && !isCopy
            && roleId == CustomerLevel.Super 
            && groupType == GroupTypes.StakeLevel) {
            return new GetGlobalStakeLevelCriteria(currency, site, appSettings.SettingStatus, systemId, userGroup);
        }

        return groupType switch
        {
            GroupTypes.SBCommission => new GetSBCommissionCriteria(custId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), isDirectMember, appSettings.SettingStatus),
            GroupTypes.Commission => new GetCommissionCriteria(custId, groupId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), appSettings.SettingStatus),
            GroupTypes.BetSetting => new GetBetSettingCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.ParlayMaxPayout => new GetMaxPayoutCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.PositionTaking => GetPositionTakingCriteria(custId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), groupId, appSettings.SettingStatus),
            GroupTypes.BetSettingWithMaxPerBall => new GetBetSettingWithMaxPerBallCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.ProductStatus => new GetProductStatusCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.StakeLevel => new GetStakeLevelCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.WinlossLimit => new GetWinlossLimitCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            _ => null
        };
    }

    private static ICriteria GetPositionTakingCriteria(int custId, byte roleId, int uplineRoleId, int groupId, byte settingStatus)
    => roleId switch
    {
        CustomerLevel.Super => new GetSuperPTCriteria(custId, groupId, settingStatus),
        CustomerLevel.Master => new GetMasterPTCriteria(custId, groupId, settingStatus),
        CustomerLevel.Agent => new GetAgentPTCriteria(custId, groupId, settingStatus),
        CustomerLevel.Member => new GetMemberPTCriteria(custId, groupId, uplineRoleId, settingStatus),
        _ => null
    };
}
