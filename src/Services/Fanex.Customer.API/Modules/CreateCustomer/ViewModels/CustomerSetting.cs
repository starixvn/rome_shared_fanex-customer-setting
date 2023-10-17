namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class CustomerSetting
{
    public GeneralInformation GeneralInfo { get; set; }

    public IEnumerable<ProductGroup> ProductGroups { get; set; }

}