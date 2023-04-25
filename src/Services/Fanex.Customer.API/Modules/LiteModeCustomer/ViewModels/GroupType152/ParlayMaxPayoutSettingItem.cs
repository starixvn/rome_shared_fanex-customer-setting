namespace Fanex.Customer.API.Modules.LiteModeCustomer.ViewModels.GroupType152;

public class ParlayMaxPayoutSettingItem : GroupItemDefault
{
    public decimal SingleMatchParlay { get; set; }
    
    public decimal MaxSingleMatchParlay { get; set; }

    public bool IsAllowUnlimitSingleMatchParlay { get; set; }

    public decimal Combo2 { get; set; }

    public decimal MaxCombo2 { get; set; }

    public bool IsAllowUnlimitCombo2 { get; set; }

    public decimal Combo35 { get; set; }

    public decimal MaxCombo35 { get; set; }

    public bool IsAllowUnlimitCombo35 { get; set; }

    public decimal ComboOver6 { get; set; }

    public decimal MaxComboOver6 { get; set; }

    public bool IsAllowUnlimitComboOver6 { get; set; }
}
