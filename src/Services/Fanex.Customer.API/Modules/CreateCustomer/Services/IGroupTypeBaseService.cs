using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface IGroupTypeBaseService
{
    Task<IEnumerable<GroupItemDefault>> Query(int uplineCustId, int productGroupId);

    // Result Update();
}
