using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType157;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    [JsonProperty(PropertyName = "MPM")]
    public decimal MaxPerMatch { get; set; }
}
