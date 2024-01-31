using Fanex.Customer.API.Modules.Password.Criteria;
using Fanex.Customer.API.Modules.Password.Model;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public interface IKickoutCustomerService
    {
        Task Kickout(KickoutCustomerParam param);
    }

    public class KickoutCustomerService : IKickoutCustomerService
    {
        private readonly IDynamicRepository dynamicRepository;

        public KickoutCustomerService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task Kickout(KickoutCustomerParam param)
        {
            if (true)
            {
                await dynamicRepository.ExecuteAsync(new KickoutCustomerCriteria(param));
            }
        }
    }
}