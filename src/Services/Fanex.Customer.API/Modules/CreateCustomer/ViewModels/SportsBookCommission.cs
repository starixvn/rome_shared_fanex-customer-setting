namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public record SportsBookCommission
{
    public int SportType { get; init; }
    public int BetType { get; init; }
    public decimal Discount { get; init; }
    public string Name { get; set; }
}