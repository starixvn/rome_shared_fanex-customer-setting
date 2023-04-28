namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class ProductGroup
{
    public int ProductGroupId { get; set; }

    public string ProductGroupName { get; set; }

    public int DisplayOrder { get; set; }

    public List<GroupType> GroupTypes { get; set; }
}