using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType150;

public class CommissionGroup : GroupItemDefault
{
    public decimal Discount { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal UplineDiscount { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal? UplineOfUplineDiscount { get; set; }

    [JsonIgnore]
    public decimal Step { get; set; }

    [JsonIgnore]
    public CommissionGroup Upline { get; set; }

    public override void MapUpline()
    {
        Upline = new CommissionGroup();
        Upline.SportTypeId = SportTypeId;
        Upline.BetTypeId = BetTypeId;
        Upline.Discount = UplineDiscount;
        if(UplineOfUplineDiscount.HasValue)
        {
            Upline.Upline = new CommissionGroup();
            Upline.Upline.SportTypeId = SportTypeId;
            Upline.Upline.BetTypeId = BetTypeId;
            Upline.Upline.Discount = UplineOfUplineDiscount.Value;
        }
    }
}
