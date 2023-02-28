namespace Fanex.Customer.API.Modules.Customer.CopyCustomer;

public class BetSetting
{
    public int SportTypeId { get; set; }

    public int BetTypeId { get; set; }

    public string BetTypeName { get; set; }

    public double MinBet { get; set; }

    public double MaxBet { get; set; }

    public double MaxPerMatch { get; set; }

    public bool IsBetSettingUpdated { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsStatusUpdated { get; set; }

    public BetSetting Upline { get; set; }

    public bool IsDataChanged()
    {
        return this.IsBetSettingUpdated || this.IsStatusUpdated;
    }
}