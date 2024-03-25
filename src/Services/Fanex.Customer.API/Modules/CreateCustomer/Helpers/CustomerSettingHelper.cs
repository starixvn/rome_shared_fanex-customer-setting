namespace Fanex.Customer.API.Modules.CreateCustomer.Helpers;

public interface ICustomerSettingHelper
{
    int GetGroupTypeByCustomerLevelAndUplineLevel(int groupType, byte roleId, byte uplineRoleId);

    int GetGroupTypeByCustomerLevel(int groupType, byte roleId);
}

public class CustomerSettingHelper : ICustomerSettingHelper
{
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

    private bool IsCrossLevel(byte roleId, byte uplineRoleId)
    => uplineRoleId - roleId > 1;
}
