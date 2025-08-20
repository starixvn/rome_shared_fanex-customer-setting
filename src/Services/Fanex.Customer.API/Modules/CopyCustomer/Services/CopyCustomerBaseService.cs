using Fanex.Customer.API.Modules.CopyCustomer.Command;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Services
{
    public abstract class CopyCustomerBaseService
    {
        protected readonly IMessageService messageService;

        protected readonly AppSettings appSettings;

        protected readonly IDynamicRepository dynamicRepository;

        protected CopyCustomerBaseService(IMessageService messageService,
            AppSettings appSettings,
            IDynamicRepository dynamicRepository)
        {
            this.messageService = messageService;
            this.appSettings = appSettings;
            this.dynamicRepository = dynamicRepository;
        }

        public abstract Task<CopyCustomerResult> Copy(CopyCustomerRequest request);
            
        public async Task<decimal> GetCashAccumCredit(int agentId)
        {
            var command = new GetCashAccumCreditCommand(agentId);
            await dynamicRepository.ExecuteAsync(command);

            return command.AccumCreditReturn;
        }
    }
}