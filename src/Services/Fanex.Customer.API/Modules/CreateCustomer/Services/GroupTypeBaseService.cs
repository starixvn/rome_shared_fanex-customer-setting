using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType151;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType152;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType155;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType156;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType158;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType160;
using Fanex.Customer.API.Modules.CreateCustomer.Helpers;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface IGroupTypeBaseService<T>
{
    Task<IEnumerable<T>> QueryForAddNew(int uplineCustId, int uplineRoleId, int productGroupId, int groupType);

    Task<IEnumerable<T>> QuerySuperForAddNew(string site, int currency, int groupId, int groupType);

    Task<IEnumerable<T>> QueryForEdit(int custId, byte roleId, byte? uplineRoleId, int groupId, int groupType);
}

public class GroupTypeBaseService<T> : IGroupTypeBaseService<T>
{
    protected readonly Dictionary<int, dynamic> criteriaMapping;

    private readonly IDynamicRepository dynamicRepository;

    private readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    public GroupTypeBaseService(IDynamicRepository dynamicRepository, 
        IOptions<AppSettings> appSettings, ICustomerSettingHelper helperService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.helperService = helperService;
    }

    public async Task<IEnumerable<T>> QueryForAddNew(int uplineCustId, int uplineRoleId, int productGroupId, int groupType)
        => await dynamicRepository.FetchAsync<T>(new GetCustomerCriteria(
            uplineCustId,
            appSettings.SettingStatus,
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

    public async Task<IEnumerable<T>> QueryForEdit(int custId, byte roleId, byte? uplineRoleId, int groupId, int groupType)
    {
        var criteria = GetQueryForEditCriteria(custId, roleId, uplineRoleId, groupId, groupType);
        if (criteria == null)
        {
            return Enumerable.Empty<T>();
        }

        return await dynamicRepository.FetchAsync<T>(criteria);
    }

    private ICriteria GetQueryForEditCriteria(int custId, byte roleId, byte? uplineRoleId, int groupId, int groupType)
    {
        return (groupType) switch
        {
            GroupTypes.SBCommission => new GetSBCommissionCriteria(custId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), appSettings.SettingStatus),
            GroupTypes.Commission => new GetCommissionCriteria(custId, groupId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), appSettings.SettingStatus),
            GroupTypes.BetSetting => new GetBetSettingCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.ParlayMaxPayout => new GetMaxPayoutCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.PositionTaking => GetPositionTakingCriteria(custId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), groupId, appSettings.SettingStatus),
            GroupTypes.ProductStatus => new GetProductStatusCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.StakeLevel => new GetStakeLevelCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.WinlossLimit => new GetWinlossLimitCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            _ => null
        };
    }

    private static ICriteria GetPositionTakingCriteria(int custId, byte roleId, int uplineRoleId, int groupId, byte settingStatus) => roleId switch
    {
        CustomerLevel.Super => new GetSuperPTCriteria(custId, groupId, settingStatus),
        CustomerLevel.Master => new GetMasterPTCriteria(custId, groupId, settingStatus),
        CustomerLevel.Agent => new GetAgentPTCriteria(custId, groupId, settingStatus),
        CustomerLevel.Member => new GetMemberPTCriteria(custId, groupId, uplineRoleId, settingStatus),
        _ => null
    };
}
