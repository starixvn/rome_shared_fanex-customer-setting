using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public interface ISiteDBHandler
    {
        Task<CustomerSiteDB> GetCustomerSiteDB(int siteDBGroupId);
    }

    public class SiteDBHandler(IDynamicRepository repository) : ISiteDBHandler
    {
        private readonly IDynamicRepository repository = repository;

        public Task<CustomerSiteDB> GetCustomerSiteDB(int siteDBGroupId)
            => repository.GetAsync<CustomerSiteDB>(new GetCustomerSiteDBCriteria(siteDBGroupId));
    }
}