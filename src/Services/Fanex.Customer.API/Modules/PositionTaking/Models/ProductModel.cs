namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class ProductModel : ActivationGroupModel
{
    public bool ShowPTIcon { get; set; }

    public string CustomerSettingLink { get; set; }

    public int MainProductId { get; set; }

    public int GroupId { get; set; }

    public bool IsNewSetting { get; set; }
}