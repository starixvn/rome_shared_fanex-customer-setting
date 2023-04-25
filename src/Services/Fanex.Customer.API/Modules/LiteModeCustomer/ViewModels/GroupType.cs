namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels;

public class GroupType
{
    public int GroupTypeId { get; set; }

    public string GroupTypeName { get; set; }

    public int GroupTypeLevel { get; set; }

    public short DisplayType { get; set; }

    public List<GroupItemDefault> GroupItems { get; set; }

    public List<GroupItemDefault> CompactGroupItems { get; set; }

    internal void ToCompact()
    {
        throw new NotImplementedException();
    }

    internal void ToDetail()
    {
        throw new NotImplementedException();
    }
}