namespace Fanex.Customer.API.Modules.CreateCustomer.Entities;

public class SettingProfile
{
    public int SportType { get; set; }

    public int BetType { get; set; }

    public bool IsLive { get; set; }

    public int GroupId { get; set; }

    public int GroupType { get; set; }

    public int? SubGroupId { get; set; }

    public short DisplayType { get; set; }

    public int SettingLevel { get; set; }

    public int SettingType { get; set; }

    public short DisplayMode { get; set; }
}
