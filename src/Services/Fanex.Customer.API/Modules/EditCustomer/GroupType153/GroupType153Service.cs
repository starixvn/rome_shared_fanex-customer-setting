using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType153;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.EditCustomer.GroupType153;

public class GroupType153Service : EditGroupTypeBaseService, IEditGroupTypeBaseService
{
    public GroupType153Service() : base(null, null) { }

    public override UpdateGroupTypeResult Validate(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        foreach(var item in groupItems)
        {
            var result = item.Validate(context.Language);
            if(!result.IsSuccessful)
            {
                return new UpdateGroupTypeResult(result);
            }
        }

        return new UpdateGroupTypeResult(Result.CreateSuccessfulResult());
    }

    public override async Task<UpdateGroupTypeResult> Update(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        var validateResult = Validate(context, productGroupId, groupTypeId, groupItems, settingStatus);
        if (!validateResult.IsSuccessful)
        {
            return validateResult;
        }

        using IObjectDb db = ObjectDbFactory.CreateInstance(GetSPName(context.RoleId, context.UplineRoleId));
        var parameters = GenerateSPParams(productGroupId, groupTypeId, context, groupItems, settingStatus);
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

    public async Task<UpdateGroupTypeResult> UpdateGroupedPTMembers(
        EditMultipleRequestContext context,
        int productGroupId,
        int groupTypeId,
        string groupedMemberIds,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        using IObjectDb db = ObjectDbFactory.CreateInstance(GetUpdateGroupedPTSPName(context.UplineRoleId));

        var parameters = GenerateUpdateGroupedPTSPParams(groupTypeId, context, groupedMemberIds, groupItems, settingStatus);
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

    private string GetUpdateGroupedPTSPName(byte uplineRoleId)
    => uplineRoleId switch
    {
        (int)CustomerLevel.Super => "Acc_CustSetting_SuperPTOnMember_153_MultiUpd",
        (int)CustomerLevel.Master => "Acc_CustSetting_MasterPTOnMember_153_MultiUpd",
        (int)CustomerLevel.Agent => "Acc_CustSetting_AgentPTOnMember_153_MultiUpd",
        _ => throw new InvalidOperationException($"Invalid UplineRoleId ({uplineRoleId})")
    };

    private Hashtable GenerateUpdateGroupedPTSPParams(
        int groupTypeId,
        EditRequestContext context,
        string groupedMemberIds,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus = 0)
        => new Hashtable {
            { "custidlist", groupedMemberIds },
            { "pt", ComposeJsonSettingParam(groupTypeId, groupItems, false) },
            { "subaccid", context.SubAccId },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "settingstatus", settingStatus}
        };

    private string GetSPName(byte roleId = 0, byte uplineRoleId = 0)
    {
        if (IsCrossLevelUpdate(roleId, uplineRoleId))
        {
            return uplineRoleId switch
            {
                (int)CustomerLevel.Super => "Acc_CustSetting_SuperPTOnMember_153_Upd",
                (int)CustomerLevel.Master => "Acc_CustSetting_MasterPTOnMember_153_Upd",
                _ => throw new InvalidOperationException($"Invalid RoleId ({roleId}) and UplineRoleId ({uplineRoleId})"),
            };
        }

        return roleId switch
        {
            (int)CustomerLevel.Super => "Acc_CustSetting_SuperPT_153_Upd",
            (int)CustomerLevel.Master => "Acc_CustSetting_MasterPT_153_Upd",
            (int)CustomerLevel.Agent => "Acc_CustSetting_AgentPT_153_Upd",
            (int)CustomerLevel.Member => "Acc_CustSetting_AgentPTOnMember_153_Upd",
            _ => throw new InvalidOperationException($"Invalid RoleId ({roleId}) and UplineRoleId ({uplineRoleId})"),
        };
    }

    private Hashtable GenerateSPParams(
        int groupId,
        int groupTypeId,
        EditRequestContext context,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus = 0)
    {
        var spParams = new Hashtable {
            { "custid", context.CustId },
            { "pt", ComposeJsonSettingParam(groupTypeId, groupItems, false) },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "settingstatus", settingStatus},
            { "groupid", groupId}
        };

        if (context.RoleId != (byte)CustomerLevel.Member)
        {
            spParams.Add("onlyminpt", ComposeJsonSettingParam(groupTypeId, groupItems, true));
        }

        if (context.RoleId != (byte)CustomerLevel.Super)
        {
            spParams.Add("subaccid", context.SubAccId);
        }

        return spParams;
    }

    private string ComposeJsonSettingParam(int groupTypeId, IEnumerable<GroupItemDefault> groupItems, bool isMinPTOnly)
    {
        if (!groupItems.Any())
        {
            throw new InvalidOperationException("Group Items cannot be empty");
        }

        var settingJSON = JArray.FromObject(groupItems.Where(item => ((PTItem)item).IsUpdateMinPtOnly == isMinPTOnly));
        return settingJSON.ToString();
    }

    private bool IsCrossLevelUpdate(byte roleId, byte uplineRoleId)
        => roleId == (int)CustomerLevel.Member && uplineRoleId != (int)CustomerLevel.Agent;
}
