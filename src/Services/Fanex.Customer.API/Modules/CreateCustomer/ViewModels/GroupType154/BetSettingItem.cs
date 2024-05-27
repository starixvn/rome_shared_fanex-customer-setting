using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType154;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    public decimal MaxPerMatch { get; set; }

    [JsonProperty(PropertyName = "MaxPerNumber", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? MaxPerBall { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal MinBetMin { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal MaxBetMax { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal MaxPerMatchMax { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal? MaxPerBallMax { get; set; }

    [JsonIgnore]
    public BetSettingItem Upline { get; set; }

    public override void MapUpline()
    {
        Upline = new BetSettingItem
        {
            SportTypeId = SportTypeId,
            BetTypeId = BetTypeId,
            MinBet = MinBetMin,
            MaxBet = MaxBetMax,
            MaxPerMatch = MaxPerMatchMax,
            MaxPerBall = MaxPerBallMax
        };
    }
}
