using System.Collections;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType154;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.EditCustomer.GroupType154;

public class GroupType154Service : EditGroupTypeBaseService, IEditGroupTypeBaseService
{
    public GroupType154Service(IMessageService messageService, ICacheRefService cacheRefService) 
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

    public override async Task<UpdateGroupTypeResult> Update(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_BetSetting_154_Upd");
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

    public override async Task<UpdateGroupTypeResult> UpdateByColumns(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        var firstBetSettingItem = groupItems.FirstOrDefault() as BetSettingItem;
        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_BetSetting_154_Upd_Multi");
        var parameters = new Hashtable {
            { "custid", context.CustId },
            { "betlimit", ComposeJsonSettingParam(groupItems) },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "systemid", context.SystemId},
            { "settingstatus", settingStatus},
            { "updminbet", firstBetSettingItem.MinBetMultipleMode},
            { "updmaxbet", firstBetSettingItem.MaxBetMultipleMode},
            { "updmpm", firstBetSettingItem.MaxPerMatchMultipleMode},
            { "updmpnumber", firstBetSettingItem.MaxPerBallMultipleMode}
        };

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
            { "betlimit", ComposeJsonSettingParam(groupItems) },
            { "subaccid", context.SubAccId },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "systemid", context.SystemId},
            { "settingstatus", settingStatus}
        };

        return spParams;
    }

    private string ComposeJsonSettingParam(IEnumerable<GroupItemDefault> groupItems)
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
