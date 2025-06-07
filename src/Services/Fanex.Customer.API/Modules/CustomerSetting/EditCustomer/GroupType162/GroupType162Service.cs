using System.Collections;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType155;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType162;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria.GroupType162;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.EditCustomer.GroupType162;

class GroupType162Service : EditGroupTypeBaseService, IEditGroupTypeBaseService
{
    protected readonly IDynamicRepository dynamicRepository;
    public GroupType162Service(IDynamicRepository dynamicRepository, IMessageService messageService, ICacheRefService cacheRefService)
        : base(messageService, cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public override UpdateGroupTypeResult Validate(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        return (UpdateGroupTypeResult)Result.CreateSuccess();
    }

    public override async Task<UpdateGroupTypeResult> Update(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        var criteria = new GetPotentialWinLimitCriteria(context.CustId, settingStatus);
        var currentWinLimit = await dynamicRepository.GetAsync<PotentialWinLimit>(criteria);

        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_PotentialWinLimit_162_Upd");
        var parameters = GenerateSPParams(context, groupItems, productGroupId, settingStatus);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), context.Language);
        var result = new UpdateGroupTypeResult(productGroupId, cacheRefService.GetProductGroupName(productGroupId), groupTypeId, StaticResources.Get($"GroupType{groupTypeId}", context.Language), Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);

        if (result.IsSuccessful)
        {
            result.Message = StaticResources.Get($"UpdateSuccess_{groupTypeId}", context.Language);
            await ResetAccumWinLossAmount(context.CustId, currentWinLimit, groupItems);
        }

        return result;
    }

    private async Task ResetAccumWinLossAmount(int custId, PotentialWinLimit currentWinLimit, IEnumerable<GroupItemDefault> groupItems)
    {
        var newWinLimit = groupItems.First() as PotentialWinLimit;
        if ((currentWinLimit == null || currentWinLimit.WinLimit == 0) && newWinLimit?.WinLimit > 0)
        {
            using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_PotentialWinLimit_162_ResetWinLossAmount");
            var parameters = new Hashtable { { "ListCustId", custId } };
            await db.ExecuteNonQueryAsync(parameters);
        }
    }

    private Hashtable GenerateSPParams(
        EditRequestContext context,
        IEnumerable<GroupItemDefault> groupItems,
        int productGroupId,
        byte settingStatus = 0)
    {
        var spParams = new Hashtable {
            { "custid", context.CustId },
            { "winlimit", ComposeJsonSettingParam(groupItems) },
            { "subaccid", context.SubAccId },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "settingstatus", settingStatus}
        };

        return spParams;
    }

    private string ComposeJsonSettingParam(IEnumerable<GroupItemDefault> groupItems)
    {
        if (!groupItems.Any())
        {
            throw new InvalidOperationException("Group Items cannot be empty");
        }

        if (groupItems.First().GetType() != typeof(PotentialWinLimit))
        {
            throw new InvalidOperationException("Item's type is not correct.");
        }

        var settingJSON = JArray.FromObject(groupItems);
        return settingJSON.ToString();
    }
}