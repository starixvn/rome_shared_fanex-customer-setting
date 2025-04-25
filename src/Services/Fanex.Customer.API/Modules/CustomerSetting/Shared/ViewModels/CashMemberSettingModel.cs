namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class CashMemberSettingModel
{
    public GeneralInfoForCashMember GeneralInfo { get; set; }

    public IEnumerable<ProductGroup> ProductGroups { get; set; }

}