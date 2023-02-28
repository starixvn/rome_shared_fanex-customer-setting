using Fanex.Customer.API.Modules.CopyCustomer.Keno.Models;

namespace Fanex.Customer.API.Modules.CopyCustomer.Keno;

public class AgentEditMemberSettingDto : CustomerSettingBase<AgentEditMemberPositionTaking>
{
    internal override void CopyToOnePositionTaking(
            AgentEditMemberPositionTaking sourceMemberPositionTaking,
            AgentEditMemberPositionTaking destinationMemberPositionTaking)
    {
        destinationMemberPositionTaking.IsUpdateA1 = destinationMemberPositionTaking.A1 != sourceMemberPositionTaking.A1;
        destinationMemberPositionTaking.A1 = sourceMemberPositionTaking.A1;
    }

    internal override void ChangeSettingWhenMultiple(AgentEditMemberPositionTaking positionTaking, bool isMultiple)
    {
        if (isMultiple)
        {
            positionTaking.IsUpdateA1 = true;
        }
    }

    public byte? SystemId { get; set; }
}