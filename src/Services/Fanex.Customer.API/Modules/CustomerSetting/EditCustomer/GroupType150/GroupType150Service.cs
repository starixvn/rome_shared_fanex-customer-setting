using System.Collections;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType150;
using Fanex.Customer.API.Modules.EditCustomer;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.CreateCustomer.EditSettings;


public class GroupType150Service : EditGroupTypeBaseService, IEditGroupTypeBaseService
{
    public GroupType150Service(IMessageService messageService, ICacheRefService cacheRefService)
        : base(messageService, cacheRefService) { }

    public override UpdateGroupTypeResult Validate(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        return (UpdateGroupTypeResult)Result.CreateSuccessfulResult();
    }

    public override async Task<UpdateGroupTypeResult> UpdateSBComm(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        string group,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_Discount150Upd");
        var parameters = GenerateSPParams(context, groupItems, group, settingStatus);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), context.Language);
        var result = new UpdateGroupTypeResult(productGroupId, cacheRefService.GetProductGroupName(productGroupId), groupTypeId, StaticResources.Get($"GroupType{groupTypeId}", context.Language), Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
        if (result.IsSuccessful)
        {
            result.Message = StaticResources.Get($"UpdateSuccess_{groupTypeId}", context.Language);
        }

        return result;
    }

    private Hashtable GenerateSPParams(
        EditRequestContext context,
        IEnumerable<GroupItemDefault> groupItems,
        string group,
        byte settingStatus = 0)
    {
        var spParams = new Hashtable {
            { "custid", context.CustId },
            { "commission", ComposeJsonSettingParam(groupItems, group) },
            { "subaccid", context.SubAccId },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "ugroup", group },
            { "settingstatus", settingStatus}
        };

        return spParams;
    }

    private string ComposeJsonSettingParam(IEnumerable<GroupItemDefault> groupItems, string group)
    {
        if (!groupItems.Any() && string.IsNullOrWhiteSpace(group))
        {
            throw new InvalidOperationException("Group Items cannot be empty");
        }

        if (groupItems.First().GetType() != typeof(CommissionGroup)
            && groupItems.First().GetType() != typeof(SuperCommissionGroup)
            && groupItems.First().GetType() != typeof(MasterCommissionGroup)
            && groupItems.First().GetType() != typeof(AgentCommissionGroup)
            && groupItems.First().GetType() != typeof(MemberCommissionGroup))
        {
            throw new InvalidOperationException("Item's type is not correct.");
        }

        var settingJSON = JArray.FromObject(groupItems);
        return settingJSON.ToString();
    }
}
