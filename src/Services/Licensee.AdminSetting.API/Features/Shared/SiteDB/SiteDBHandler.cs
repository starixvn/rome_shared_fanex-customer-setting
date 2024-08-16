using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Shared.SiteDB
{
    public interface ISiteDBHandler
    {
        Task<SiteDBConfig> GetBySiteDBGroupId(int siteDBGroupId);

        Task<SiteDBConfig> GetBySite(string site);
    }

    public class SiteDBHandler(IDynamicRepository repository) : ISiteDBHandler
    {
        private readonly IDynamicRepository repository = repository;

        public Task<SiteDBConfig> GetBySiteDBGroupId(int siteDBGroupId)
            => repository.GetAsync<SiteDBConfig>(new GetCustomerSiteDBCriteria(siteDBGroupId));

        public Task<SiteDBConfig> GetBySite(string site)
            => repository.GetAsync<SiteDBConfig>(new GetSiteDBConfigCriteria(site));
    }
}