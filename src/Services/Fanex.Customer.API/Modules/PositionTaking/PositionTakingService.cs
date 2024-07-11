using Fanex.Customer.API.Modules.PositionTaking.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.PositionTaking;

public interface IPositionTakingService
{
    IList<int> GetPageSizeList();

    Task<PositionTakingViewModel> GetPT(PositionTakingRequest request);
}

public class PositionTakingService : IPositionTakingService
{
    public IList<int> GetPageSizeList()
    {
        return new List<int> { 10, 50, 100, 500 };
    }

    public async Task<PositionTakingViewModel> GetPT(PositionTakingRequest request)
    {
        throw new NotImplementedException();
    }
}