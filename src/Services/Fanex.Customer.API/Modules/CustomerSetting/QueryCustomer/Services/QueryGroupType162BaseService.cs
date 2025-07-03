using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType162;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType162;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Fanex.Customer.API.Modules.QueryCustomer.Services;

public class QueryGroupType162BaseService<T> : QueryGroupTypeBaseService<T>
{
    public QueryGroupType162BaseService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ICustomerSettingHelper helperService) : base(dynamicRepository, appSettings, helperService)
    {
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "<Pending>")]
    public new async Task<IEnumerable<T>> QueryForEdit(
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
        var criteria = new GetPotentialWinLimitCriteria(custId, appSettings.SettingStatus);
        var potentialWinLimit = await dynamicRepository.GetAsync<PotentialWinLimit>(criteria);

        if (potentialWinLimit != null)
        {
            var amount = await dynamicRepository.GetAsync<PotentialWinLimitAmount>(new GetPotentialWinLimitAmountCriteria(custId));
            if (amount != null)
            {
                potentialWinLimit.AccumWinLoss = amount.AccumWinLoss;
                potentialWinLimit.AccumToWin = amount.AccumToWin;
            }

            return new[] { potentialWinLimit }.Cast<T>();
        }

        return Enumerable.Empty<T>();
    }
}
