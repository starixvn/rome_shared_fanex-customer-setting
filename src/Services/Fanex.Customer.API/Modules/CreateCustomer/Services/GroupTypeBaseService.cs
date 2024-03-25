using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType150;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType153;
using Fanex.Customer.API.Modules.CreateCustomer.Helpers;
using Fanex.Data.Repository;

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

    public GroupTypeBaseService(IDynamicRepository dynamicRepository, AppSettings appSettings, ICustomerSettingHelper helperService)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        this.appSettings = appSettings;
        this.helperService = helperService;
        criteriaMapping = new Dictionary<int, dynamic>(){
            { 150, new GetCommissionCriteria() },
            { 153_4, new GetSuperPTCriteria() },
            { 153_3, new GetMasterPTCriteria() },
            { 153_2, new GetAgentPTCriteria() },
            { 153_1, new GetMemberPTCriteria() }
        };
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

        return await dynamicRepository.FetchAsync<T>(criteria);

    }

    private ICriteria GetQueryForEditCriteria(int custId, byte roleId, byte? uplineRoleId, int groupId, int groupType)
    {
        dynamic criteria = null;
        if (criteriaMapping.ContainsKey(groupType))
            criteria = criteriaMapping[groupType];
        var key = helperService.GetGroupTypeByCustomerLevel(groupType, roleId);
        if (criteriaMapping.ContainsKey(key))
            criteria = criteriaMapping[key];

        if (criteria is null)
            criteria = new GetCustomerForUpdateCriteria(
                custId,
                appSettings.SettingStatus,
                roleId,
                groupId,
                groupType);
        else
        {
            criteria.CustId = custId;
            if (groupType == 150)
            {
                criteria.RoleId = roleId;
            }

            if (roleId == 1 && groupType == 150)
            {

                criteria.UplineRoleId = uplineRoleId.HasValue ? uplineRoleId.Value : roleId + 1;
            }

            if (roleId == 1 && groupType == 153)
            {
                criteria.RoleId = uplineRoleId.HasValue ? uplineRoleId.Value : roleId + 1;
            }

            criteria.SettingStatus = appSettings.SettingStatus;
        }

        return criteria;
    }
}
