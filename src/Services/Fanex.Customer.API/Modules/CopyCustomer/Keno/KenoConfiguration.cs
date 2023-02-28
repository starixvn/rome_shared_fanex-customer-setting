namespace Fanex.Customer.API.Modules.CopyCustomer.Keno;

public class KenoConfiguration
{
    public const string Keno = "Keno";

    public bool ShowSetting { get; set; }

    public int[] ExcludeBetTypes { get; set; }

    public int[] ExcludeSettingBetTypes { get; set; }
}