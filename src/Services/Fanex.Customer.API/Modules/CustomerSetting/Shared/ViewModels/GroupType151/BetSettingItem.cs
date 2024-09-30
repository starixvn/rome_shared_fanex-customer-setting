using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType151;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    [JsonProperty(PropertyName = "MPM")]
    public decimal MaxPerMatch { get; set; }

    [JsonProperty(PropertyName = "MaxPayout", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? MaxPayoutPerMatch { get; set; }

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
    public decimal MaxPayoutPerMatchMax { get; set; }

    [JsonIgnore]
    public BetSettingItem Upline { get; set; }

    public bool MinBetMultipleMode { get; set; }
    public bool MaxBetMultipleMode { get; set; }
    public bool MaxPerMatchMultipleMode { get; set; }
    public bool MaxPayoutPerMatchMultipleMode { get; set; }

    public override void MapUpline()
    {
        Upline = new BetSettingItem
        {
            SportTypeId = SportTypeId,
            BetTypeId = BetTypeId,
            MinBet = MinBetMin,
            MaxBet = MaxBetMax,
            MaxPerMatch = MaxPerMatchMax,
            MaxPayoutPerMatch = MaxPayoutPerMatchMax
        };
    }
}
