using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ISiteService
{
    Task<IEnumerable<Site>> GetCreditSites(int userId);
}

public class SiteService : ISiteService
{
    private readonly IDynamicRepository dynamicRepository;

    public SiteService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
    }

    public async Task<IEnumerable<Site>> GetCreditSites(int userId)
        => (await this.dynamicRepository.FetchAsync<Site>(new GetCreditSitesCriteria(userId))).Where(item => item.IsLicensee == false);
}
