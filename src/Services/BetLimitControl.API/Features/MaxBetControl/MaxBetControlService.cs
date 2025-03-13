using BetLimitControl.API.Features.MaxBetControl.Criteria;
using BetLimitControl.API.Features.MaxBetControl.Models;
using Fanex.Data.Repository;

namespace BetLimitControl.API.Features.MaxBetControl
{
    public interface IMaxBetControlService
    {
        Task<IEnumerable<int>> GetSites();

        Task<IEnumerable<CustomerMultiplier>> GetCustomerMultipliersBySite(int siteId);
    }

    public class MaxBetControlService(IDynamicRepository dynamicRepository, IConfiguration configuration) : IMaxBetControlService
    {
        private readonly IDynamicRepository dynamicRepository = dynamicRepository;
        private readonly bool IsTest = configuration.GetValue<bool>("IsTest");

        public Task<IEnumerable<CustomerMultiplier>> GetCustomerMultipliersBySite(int siteId)
            => dynamicRepository.FetchAsync<CustomerMultiplier>(new GetCustomerMultipliersBySiteCriteria(siteId, IsTest));

        public Task<IEnumerable<int>> GetSites()
            => dynamicRepository.FetchAsync<int>(new GetMaxBetPlayerSitesCriteria(IsTest));
    }
}