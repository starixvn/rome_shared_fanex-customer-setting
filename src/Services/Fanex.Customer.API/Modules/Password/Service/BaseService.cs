using Fanex.Customer.API.Modules.Password.Criteria;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public class BaseService
    {

        protected readonly IDynamicRepository dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        protected async Task<Result> ExecuteCommand(BaseCommand command)
        {
            await dynamicRepository.ExecuteAsync(command);

            return new Result
            {
                Code = command.ErrCode,
                Message = command.ErrMsg
            };
        }
    }
}