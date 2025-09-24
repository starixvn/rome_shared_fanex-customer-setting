using System.Data;
using System.Linq;
using Fanex.Common;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria.EditSettings.GroupType158;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType150;
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
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType162;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.QueryCustomer.Services;

public interface IQueryGroupTypeBaseService<T>
{
    Task<IEnumerable<T>> QueryForAddNew(int uplineCustId, int uplineRoleId, int productGroupId, int groupType, bool? isDirectMember);

    Task<IEnumerable<T>> QuerySuperForAddNew(string site, int currency, int groupId, int groupType);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "<Pending>")]
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
        int? agentId);
}

public class QueryGroupTypeBaseService<T> : IQueryGroupTypeBaseService<T>
{
    protected readonly Dictionary<int, dynamic> criteriaMapping;

    protected readonly IDynamicRepository dynamicRepository;

    protected readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    public QueryGroupTypeBaseService(IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings, ICustomerSettingHelper helperService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.helperService = helperService;
    }

    public async Task<IEnumerable<T>> QueryForAddNew(int uplineCustId, int uplineRoleId, int productGroupId, int groupType, bool? isDirectMember)
        => await dynamicRepository.FetchAsync<T>(uplineRoleId == CustomerLevel.Agent
        ? new GetAgentCriteria(
            uplineCustId,
            appSettings.SettingStatus,
            uplineRoleId,
            productGroupId,
            groupType)
        : new GetSuperMasterCriteria(
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
        int? agentId)
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
            agentId);

        if (criteria == null)
        {
            return Enumerable.Empty<T>();
        }

        if (groupType == GroupTypes.SBCommission || groupType == GroupTypes.Commission)
        {
            var result = new List<GroupItemDefault>();
            using IObjectDb objDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            var spParams = groupType == GroupTypes.SBCommission ?
                (criteria as GetSBCommissionCriteria).GetParameters() :
                (criteria as GetCommissionCriteria).GetParameters();
            objDb.AssignParamValues(spParams);
            var reader = await objDb.ExecuteReaderAsync();
            while (reader.Read())
            {
                CommissionGroup item = MappingData(reader, roleId, uplineRoleId, agentId);
                result.Add(item);
            }

            return result.Cast<T>();
        }

        return await dynamicRepository.FetchAsync<T>(criteria);
    }

    private static CommissionGroup MappingData(IDataReader reader, int roleId, int? uplineRoleId, int? agentId)
    {
        var discountConfig = GetDiscountConfiguration(roleId, uplineRoleId, agentId);
        
        return new CommissionGroup
        {
            SportTypeId = Converter.ToInt(reader["SportType"]),
            BetTypeId = Converter.ToInt(reader["BetType"]),
            Discount = Converter.ToDecimal(reader[discountConfig.DiscountField]),
            UplineDiscount = Converter.ToDecimal(reader[discountConfig.UplineDiscountField]),
            UplineOfUplineDiscount = discountConfig.UplineOfUplineDiscountField != null 
                ? Converter.ToDecimal(reader[discountConfig.UplineOfUplineDiscountField]) 
                : null,
            Step = Converter.ToDecimal(reader["IntervalComm"]),
        };
    }

    private static DiscountConfiguration GetDiscountConfiguration(int roleId, int? uplineRoleId, int? agentId)
    {
        // Handle special agent cases first
        if (agentId.HasValue)
        {
            return agentId.Value switch
            {
                -3 => new DiscountConfiguration("PlayerDiscount", "MasterDiscount", null),
                -4 => new DiscountConfiguration("PlayerDiscount", "SuperDiscount", null),
                _ => GetRoleBasedConfiguration(roleId, uplineRoleId)
            };
        }

        return GetRoleBasedConfiguration(roleId, uplineRoleId);
    }

    private static DiscountConfiguration GetRoleBasedConfiguration(int roleId, int? uplineRoleId)
    {
        return roleId switch
        {
            CustomerLevel.Member => new DiscountConfiguration(
                "PlayerDiscount", 
                "AgentDiscount", 
                uplineRoleId == CustomerLevel.Super ? "MasterDiscount" : null),
            CustomerLevel.Agent => new DiscountConfiguration("AgentDiscount", "MasterDiscount", null),
            CustomerLevel.Master => new DiscountConfiguration("MasterDiscount", "SuperDiscount", null),
            CustomerLevel.Super => new DiscountConfiguration("SuperDiscount", "DefaultMaxDiscount", null),
            _ => throw new ArgumentException($"Unknown role ID: {roleId}", nameof(roleId))
        };
    }

    private record DiscountConfiguration(string DiscountField, string UplineDiscountField, string UplineOfUplineDiscountField);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "<Pending>")]
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
        int? agentId)
    {
        if (isMultipleMode
            && !isCopy
            && roleId == CustomerLevel.Super
            && groupType == GroupTypes.StakeLevel)
        {
            return new GetGlobalStakeLevelCriteria(currency, site, appSettings.SettingStatus, systemId, userGroup);
        }

        return groupType switch
        {
            GroupTypes.SBCommission => new GetSBCommissionCriteria(custId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), agentId, appSettings.SettingStatus),
            GroupTypes.Commission => new GetCommissionCriteria(custId, groupId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), agentId, appSettings.SettingStatus),
            GroupTypes.BetSetting => new GetBetSettingCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.ParlayMaxPayout => new GetMaxPayoutCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.PositionTaking => GetPositionTakingCriteria(custId, roleId, uplineRoleId.HasValue ? uplineRoleId.Value : (roleId + 1), groupId, appSettings.SettingStatus),
            GroupTypes.BetSettingWithMaxPerBall => new GetBetSettingWithMaxPerBallCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.ProductStatus => new GetProductStatusCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.StakeLevel => new GetStakeLevelCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.WinlossLimit => new GetWinlossLimitCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.WinLimit => new GetWinLimitCriteria(custId, appSettings.SettingStatus, roleId, groupId, groupType),
            GroupTypes.PotentialWinLimit => new GetPotentialWinLimitCriteria(custId, appSettings.SettingStatus),
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
