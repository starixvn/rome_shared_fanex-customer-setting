using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType157;

public class BetSettingItem : GroupItemDefault
{
    public decimal MinBet { get; set; }

    public decimal MaxBet { get; set; }

    [JsonProperty(PropertyName = "MPM")]
    public decimal MaxPerMatch { get; set; }
    
    public bool MinBetMultipleMode { get; set; }
    public bool MaxBetMultipleMode { get; set; }
    public bool MaxPerMatchMultipleMode { get; set; }
}
