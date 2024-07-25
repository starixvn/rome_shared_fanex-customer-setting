namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class ProductGroup
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<GroupType> GroupTypes { get; set; }
}