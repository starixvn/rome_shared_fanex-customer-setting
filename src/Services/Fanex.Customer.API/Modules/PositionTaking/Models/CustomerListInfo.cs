namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class CustomerListInfo
{
    public int CustId { get; set; }

    public string CustName { get; set; }

    public int RoleId { get; set; }

    public bool Closed { get; set; }

    public bool Enabled { get; set; }

    public bool Suspended { get; set; }

    public bool IsCashMember { get; set; }

    public CustomerClosedReason ClosedReason { get; set; }

    public CustomerLockedAccess LockedAccess { get; set; }
}