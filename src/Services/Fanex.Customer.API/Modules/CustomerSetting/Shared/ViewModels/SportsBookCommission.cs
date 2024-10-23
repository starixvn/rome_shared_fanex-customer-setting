namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public record SportsBookCommission
{
    public int SportType { get; init; }
    public int BetType { get; init; }
    public decimal Discount { get; init; }
    public string Name { get; set; }
}