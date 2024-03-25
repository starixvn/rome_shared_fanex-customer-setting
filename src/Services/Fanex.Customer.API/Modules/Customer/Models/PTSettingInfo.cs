namespace Fanex.Customer.API.Modules.Customer.Models;

public record PTSettingInfoDto(string TypeDesc, int TypeId)
{
    public required string BettypeName { get; set; }

    public required string DisplayTypeName { get; set; }
}

public class PTSettingInfo
{
    public int TypeDesc { get; set; }

    public string BettypeName { get; set; }

    public List<BettypeInfo> Bettypes { get; set; }
}

public class BettypeInfo
{
    public int TypeId { get; set; }

    public string DisplayTypeName { get; set; }
}