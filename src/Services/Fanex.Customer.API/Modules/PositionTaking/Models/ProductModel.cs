namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class ProductModel : ActivationGroupModel
{
    public string ViewPTLink { get; set; }

    public string ViewCommLink { get; set; }

    public bool HasReport { get; set; }

    public bool ShowPTIcon { get; set; }

    public string CustomerSettingLink { get; set; }

    public int MainProductId { get; set; }

    public int GroupId { get; set; }

    public bool IsNewSetting { get; set; }
}