using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels.GroupType155;

public class WinLossLimitItem : GroupItemDefault
{
    [JsonProperty(PropertyName = "MaxWinLimit")]
    public decimal WinLimit { get; set; }

    [JsonIgnore]
    public decimal MaxWinRange { get; set; }

    [JsonProperty(PropertyName = "MaxLossLimit")]
    public decimal LossLimit { get; set; }

    [JsonIgnore]
    public decimal MaxLossRange { get; set; }

    [JsonIgnore]
    public decimal MinLossRange { get; set; }

    public bool IsAutoReset { get; set; }

    [JsonIgnore]
    public bool IsAutoResetUpline { get; set; }
}
