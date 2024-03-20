using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType151;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.EditCustomer.GroupType152;

public class GroupType151Service : EditGroupTypeBaseService, IEditGroupTypeBaseService
{
    public GroupType151Service() : base(null, null) { }

    public override UpdateGroupTypeResult Validate(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        return (UpdateGroupTypeResult)Result.CreateSuccessfulResult();
    }

    public override async Task<UpdateGroupTypeResult> Update(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_BetSetting151Upd");
        var parameters = GenerateSPParams(groupTypeId, context, groupItems, settingStatus);
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
        int groupTypeId,
        EditRequestContext context,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus = 0)
    {
        var spParams = new Hashtable {
            { "custid", context.CustId },
            { "betsetting", ComposeJsonSettingParam(groupTypeId, groupItems) },
            { "subaccid", context.SubAccId },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "systemid", context.SystemId},
            { "settingstatus", settingStatus}
        };

        return spParams;
    }

    private string ComposeJsonSettingParam(int groupTypeId, IEnumerable<GroupItemDefault> groupItems)
    {
        if(!groupItems.Any())
        {
            throw new InvalidOperationException("Group Items cannot be empty");
        }

        if (groupItems.First().GetType() != typeof(BetSettingItem))
        {
            throw new InvalidOperationException();
        }

        var settingJSON = JArray.FromObject(groupItems);
        return settingJSON.ToString();
    }
}
