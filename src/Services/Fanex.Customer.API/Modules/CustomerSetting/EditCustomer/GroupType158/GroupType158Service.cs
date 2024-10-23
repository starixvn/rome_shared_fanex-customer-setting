using System.Collections;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType158;
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


public class GroupType158Service : EditGroupTypeBaseService, IEditGroupTypeBaseService
{
    public GroupType158Service(IMessageService messageService, ICacheRefService cacheRefService) 
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
        using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustSetting_StakeLevel_158_Upd");
        var parameters = GenerateSPParams(context, groupItems, settingStatus);
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
        byte settingStatus = 0)
    {
        var spParams = new Hashtable {
            { "custid", context.CustId },
            { "stakelevel", ComposeJsonSettingParam(groupItems) },
            { "subaccid", context.SubAccId },
            { "userid", context.ActorId },
            { "ip", context.IP },
            { "systemid", context.SystemId },
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

        if (groupItems.First().GetType() != typeof(StakeLevelItem))
        {
            throw new InvalidOperationException("Item's type is not correct.");
        }

        var settingJSON = new JArray();
        int prevSportTypeId = 0;
        int prevBetTypeId = 0;
        string stakeLevelList = string.Empty;
        if (groupItems.Any())
        {
            foreach (var item in groupItems.Cast<StakeLevelItem>())
            {
                if (prevSportTypeId != item.SportTypeId && prevBetTypeId != item.BetTypeId)
                {
                    if (prevSportTypeId != 0 && prevBetTypeId != 0)
                    {
                        settingJSON.Add(new JObject
                        {
                            new JProperty("SportType", prevSportTypeId),
                            new JProperty("BetType", prevBetTypeId),
                            new JProperty("StakeLevelList", stakeLevelList.Trim(','))
                        });
                    }

                    prevSportTypeId = item.SportTypeId;
                    prevBetTypeId = item.BetTypeId;
                    stakeLevelList = string.Empty;
                }

                stakeLevelList += item.IsChecked ? (item.StakeLevelId + ",") : string.Empty;
            }
        }

        if (prevSportTypeId != 0 && prevBetTypeId != 0)
        {
            settingJSON.Add(new JObject
            {
                new JProperty("SportType", prevSportTypeId),
                new JProperty("BetType", prevBetTypeId),
                new JProperty("StakeLevelList", stakeLevelList.Trim(','))
            });
        }

        return settingJSON.ToString();
    }
}
