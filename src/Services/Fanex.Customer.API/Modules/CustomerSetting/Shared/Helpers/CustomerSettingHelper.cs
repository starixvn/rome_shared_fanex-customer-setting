using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;

public interface ICustomerSettingHelper
{
    string CreateSettingTypesJson(IEnumerable<ProductGroup> productGroups, bool isFullMode);

    string ObjectToXml(object o);

    int GetGroupTypeByCustomerLevelAndUplineLevel(int groupType, byte roleId, byte uplineRoleId);

    int GetGroupTypeByCustomerLevel(int groupType, byte roleId);

    bool IsGroupTypeByLevel(int groupType);

    bool IsPTGroupType(int groupType);

    bool IsCrossLevel(byte roleId, byte uplineRoleId);
}

public class CustomerSettingHelper : ICustomerSettingHelper
{
    public string CreateSettingTypesJson(IEnumerable<ProductGroup> productGroups, bool isFullMode)
    {
        var groupItems = productGroups.Where(p => !isFullMode || p.Id == 1)
        .SelectMany(p => p.GroupTypes.SelectMany(gi => gi.GroupItems.Select(i => new { GroupTypeId = gi.Id, GroupItem = i })));
        var items = groupItems.GroupBy(g => g.GroupTypeId)
        .Select(t => new JProperty(t.Key.ToString(), JArray.FromObject(t.Select(i => i.GroupItem))));
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
    
    public int GetGroupTypeByCustomerLevelAndUplineLevel(int groupType, byte roleId, byte uplineRoleId)
    {
        if (groupType == 153 && IsCrossLevel(roleId, uplineRoleId))
        {
            return GetGroupTypeByCustomerLevel(groupType, roleId) * 10 + uplineRoleId;
        }

        return GetGroupTypeByCustomerLevel(groupType, roleId);
    }

    public int GetGroupTypeByCustomerLevel(int groupType, byte roleId)
    => groupType * 10 + roleId;

    public bool IsGroupTypeByLevel(int groupType)
    => groupType == 153 || groupType == 150 || groupType == 156;
    
    public bool IsPTGroupType(int groupType)
    => groupType == 153;

    public bool IsCrossLevel(byte roleId, byte uplineRoleId)
    => uplineRoleId - roleId > 1;
}
