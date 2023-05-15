namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class GroupType
{
    public int Id { get; set; }

    public string Name { get; set; }

    public short DisplayType { get; set; }

    public int? SubGroupId { get; set; }

    public string SubGroupName { get; set; }

    public IEnumerable<GroupItemDefault> GroupItems { get; set; }
}