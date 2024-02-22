using Fanex.Customer.API.Modules.Customer.Criteria;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer
{
    public interface ILanguageService
    {
        Task<Result> UpdateUsageLanguage(UsageLanguageRequest request);
    }

    public class LanguageService : ILanguageService
    {
        private readonly IDynamicRepository dynamicRepository;

        public LanguageService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<Result> UpdateUsageLanguage(UsageLanguageRequest request)
        {
            var result = await dynamicRepository.ExecuteAsync(new UpdateUsageLanguageCommand(request.CustId, request.SubAccId, request.LanguageUsage, request.IP));

            return new Result();
        }
    }
}