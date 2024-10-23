using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels.GroupType152;

public class ParlayMaxPayoutSettingItem : GroupItemDefault
{
    [JsonProperty(PropertyName="SingleMP")]
    public decimal SingleMatchParlay { get; set; }

    [JsonIgnore]
    public bool IsAllowUnlimitSingleMatchParlay { get; set; }

    [JsonProperty(PropertyName="Combo2")]
    public decimal Combo2 { get; set; }

    [JsonIgnore]
    public bool IsAllowUnlimitCombo2 { get; set; }

    [JsonProperty(PropertyName="Combo35")]
    public decimal Combo35 { get; set; }

    [JsonIgnore]
    public bool IsAllowUnlimitCombo35 { get; set; }

    [JsonProperty(PropertyName="Combo6")]
    public decimal ComboOver6 { get; set; }

    [JsonIgnore]
    public bool IsAllowUnlimitComboOver6 { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal? UplineSingleMatchParlay { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal? UplineCombo2 { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal? UplineCombo35 { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public decimal? UplineComboOver6 { get; set; }

    [JsonIgnore]
    public ParlayMaxPayoutSettingItem Upline { get; set; }

    public override void MapUpline()
    {
        if(UplineSingleMatchParlay.HasValue)
        {
            Upline = new ParlayMaxPayoutSettingItem
            {
                SportTypeId = SportTypeId,
                BetTypeId = BetTypeId,
                SingleMatchParlay = UplineSingleMatchParlay.Value,
                Combo2 = UplineCombo2.Value,
                Combo35 = UplineCombo35.Value,
                ComboOver6 = UplineComboOver6.Value,
                IsAllowUnlimitSingleMatchParlay = IsAllowUnlimitSingleMatchParlay,
                IsAllowUnlimitCombo2 = IsAllowUnlimitCombo2,
                IsAllowUnlimitCombo35 = IsAllowUnlimitCombo35,
                IsAllowUnlimitComboOver6 = IsAllowUnlimitComboOver6
            };
        }
    }
}
