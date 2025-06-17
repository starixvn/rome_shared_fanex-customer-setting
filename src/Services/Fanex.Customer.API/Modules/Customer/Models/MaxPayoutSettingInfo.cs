namespace Fanex.Customer.API.Modules.Customer.Models;

public class MaxPayoutSettingInfo
{
    public MaxPayoutSport MaxPayoutSport { get; set; }
}

public class MaxPayoutSport
{
    public string SportName { get; set; }

    public string[] BettypeName { get; set; }
}