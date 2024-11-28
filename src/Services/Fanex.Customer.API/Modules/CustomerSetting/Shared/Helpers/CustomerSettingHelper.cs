using System.Globalization;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType158;
using Fanex.Customer.API.Shared.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;

public interface ICustomerSettingHelper
{
    string CreateSettingTypesJson(IEnumerable<ProductGroup> productGroups, bool isFullMode);

    string ObjectToXml(object o);

    int GetGroupTypeByCustomerLevelAndUplineLevel(int groupType, byte roleId, byte uplineRoleId, bool? isDirectMember);

    int GetGroupTypeByCustomerLevel(int groupType, byte roleId);

    bool IsGroupTypeByLevel(int groupType);

    bool IsPTGroupType(int groupType);

    bool IsSBCommGroupType(int groupType);

    bool IsCrossLevel(byte roleId, byte uplineRoleId);
}

public class CustomerSettingHelper : ICustomerSettingHelper
{
    public string CreateSettingTypesJson(IEnumerable<ProductGroup> productGroups, bool isFullMode)
    {
        var groupItems = productGroups.Where(p => !isFullMode || p.Id == 1)
        .SelectMany(p => p.GroupTypes.SelectMany(gi => gi.GroupItems.Select(i => new { GroupTypeId = gi.Id, GroupItem = i })));
        var items = groupItems.GroupBy(g => g.GroupTypeId)
        .Select(t => new JProperty(t.Key.ToString(), IsGroupStakeLevelGroupType(t.Key)? ComposeGroupType158JsonSettingParam(t.Select(i => i.GroupItem)) : JArray.FromObject(t.Select(i => i.GroupItem))));
        var settingJSON = new JObject(items);
        return settingJSON.ToString();
    }

    public string ObjectToXml(object o)
    {
        if (o == null)
        {
            return string.Empty;
        }

        var jsonObj = JsonConvert.SerializeObject(o);
        var wrappedDocument = string.Format("{{ item: {0} }}", jsonObj);
        return JsonConvert.DeserializeXmlNode(wrappedDocument)?.OuterXml.Replace("<item>", string.Empty).Replace("</item>", string.Empty);
    }

    public int GetGroupTypeByCustomerLevelAndUplineLevel(int groupType, byte roleId, byte uplineRoleId, bool? isDirectMember)
    {
        var result = GetGroupTypeByCustomerLevel(groupType, roleId);
        if (IsCrossLevel(roleId, uplineRoleId))
        {
            result = result * 10 + uplineRoleId;
            return isDirectMember.GetValueOrDefault() ? result * 10 + 1 : result;
        }

        return isDirectMember.GetValueOrDefault() ? result * 10 + 1 : result;
    }

    public int GetGroupTypeByCustomerLevel(int groupType, byte roleId)
    => groupType * 10 + roleId;

    public bool IsGroupTypeByLevel(int groupType)
    => groupType == GroupTypes.PositionTaking || groupType == GroupTypes.SBCommission || groupType == GroupTypes.Commission;

    public bool IsPTGroupType(int groupType)
    => groupType == GroupTypes.PositionTaking;

    public bool IsSBCommGroupType(int groupType)
    => groupType == 150;

    public bool IsCrossLevel(byte roleId, byte uplineRoleId)
    => uplineRoleId - roleId > 1;

    public bool IsGroupStakeLevelGroupType(int groupType)
    => groupType == GroupTypes.StakeLevel;

    public JArray ComposeGroupType158JsonSettingParam(IEnumerable<GroupItemDefault> groupItems)
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

        return settingJSON;
    }
}
