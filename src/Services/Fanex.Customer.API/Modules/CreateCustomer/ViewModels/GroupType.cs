namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class GroupType
{
    public int Id { get; set; }

    public string Name { get; set; }

    public short DisplayType { get; set; }

    public IEnumerable<SubGroup> SubGroups { get; set; }
}