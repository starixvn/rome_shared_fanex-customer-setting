namespace Fanex.Customer.API.Modules.PositionTaking.Models;

public class PositionTakingEntityList
{
    public IEnumerable<PositionTakingEntity> CustPT { get; set; }

    public int TotalPages { get; set; }
}