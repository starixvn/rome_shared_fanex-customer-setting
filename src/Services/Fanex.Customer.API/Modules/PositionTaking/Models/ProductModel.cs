namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class ProductModel : ActivationGroupModel
{
    public bool ShowPTIcon { get; set; }

    public int GroupId { get; set; }

    public bool IsNewSetting { get; set; }
}