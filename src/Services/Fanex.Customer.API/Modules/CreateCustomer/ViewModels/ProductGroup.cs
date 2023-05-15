namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class ProductGroup
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<GroupType> GroupTypes { get; set; }
}