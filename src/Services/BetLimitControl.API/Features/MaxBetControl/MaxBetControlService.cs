using BetLimitControl.API.Features.MaxBetControl.Criteria;
using BetLimitControl.API.Features.MaxBetControl.Models;
using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl
{
    public interface IMaxBetControlService
    {
        Task<IEnumerable<string>> GetSites();

        Task<IEnumerable<CustomerMultiplier>> GetCustomerMultipliersBySite(int siteId);
    }

    public class MaxBetControlService(IDynamicRepository dynamicRepository) : IMaxBetControlService
    {
        private readonly IDynamicRepository dynamicRepository = dynamicRepository;

        public Task<IEnumerable<CustomerMultiplier>> GetCustomerMultipliersBySite(int siteId)
            => dynamicRepository.FetchAsync<CustomerMultiplier>(new GetCustomerMultipliersBySiteCriteria(siteId));

        public Task<IEnumerable<string>> GetSites()
            => dynamicRepository.FetchAsync<string>(new GetMaxBetPlayerSitesCriteria());
    }
}