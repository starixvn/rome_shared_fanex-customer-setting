using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType154;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    [JsonProperty(PropertyName = "MPM")]
    public decimal MaxPerMatch { get; set; }

    [JsonProperty(PropertyName = "MaxPerNumber", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? MaxPerBall { get; set; }

    public int SubType { get; set; }

    public BetSettingItem Upline { get; set; }
}
