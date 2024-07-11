namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class PositionTakingByGroupModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Value { get; set; }

    public List<PositionTakingByGroupModel> PositionTakingByGroups { get; set; }
}