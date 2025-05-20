using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType161;

public class WinLimitItem : GroupItemDefault
{
    [JsonProperty(PropertyName = "MaxWinLimit")]
    public decimal WinLimit { get; set; }

    [JsonIgnore]
    public decimal MaxWinRange { get; set; }
    
    [JsonIgnore]
    public decimal MinWinRange => 0.1m;

    public bool IsAutoReset { get; set; }

    [JsonIgnore]
    public bool IsAutoResetUpline { get; set; }
}
