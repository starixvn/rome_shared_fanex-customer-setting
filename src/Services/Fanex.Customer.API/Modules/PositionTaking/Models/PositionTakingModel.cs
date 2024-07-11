namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class PositionTakingModel
{
    public CustomerListInfo CustomerInfo { get; set; }

    public int GroupIndex { get; set; }

    public List<PositionTakingModel> UplinePT { get; set; }

    public List<PositionTakingByGroupModel> PositionTakingByGroups { get; set; }
}