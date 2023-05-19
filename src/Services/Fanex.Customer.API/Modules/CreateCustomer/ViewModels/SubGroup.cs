namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class SubGroup
{
    public int? Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<GroupItemDefault> GroupItems { get; set; }
}
