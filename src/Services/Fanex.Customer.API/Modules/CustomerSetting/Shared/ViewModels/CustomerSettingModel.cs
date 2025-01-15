namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class CustomerSettingModel
{
    public string Group { get; set; }
    
    public GeneralInformation GeneralInfo { get; set; }

    public IEnumerable<ProductGroup> ProductGroups { get; set; }

}