namespace Fanex.Customer.API.Modules.Customer.Models;

public class MaxPayoutSettingInfo
{
    public MaxPayoutSport MaxPayoutSport { get; set; }

    public IEnumerable<SabaSport> SabaSport { get; set; }
    
    public bool IsSpecialPlatform { get; set; }
}

public class SabaSport
{
    public string SettingId { get; set; }

    public string SettingName { get; set; }

    public string SettingIdFollowed { get; set; }

    public string SettingFollowedName { get; set; }
}

public class MaxPayoutSport
{
    public string SportName { get; set; }

    public string[] BettypeName { get; set; }
}