namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class CustomerSetting
{
    public GeneralInformation GeneralInfo { get; set; }

    public Dictionary<int, ProductGroup> ProductGroups { get; set; }

}