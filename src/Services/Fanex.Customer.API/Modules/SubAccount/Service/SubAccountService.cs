using Fanex.Customer.API.Modules.SubAccount.Command;
using Fanex.Customer.API.Modules.SubAccount.Model;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Service
{
    public interface ISubAccountService
    {
        public Task<IEnumerable<SubAccountModel>> GetSubAccounts(int custId);

        public Task<SubAccountModel> GetSubAccount(int custId, int subAccId);

        public Task<Result> UpdateSubAccountInfo(UpdateSubAccountInfoModel subAccountModel);
        
        public Task<Result> UpdateSubAccountPageAccess(UpdateSubAccountPageAccessModel subAccountModel);

        public Task<Result> AddSubAccount(AddSubAccountModel subAccountModel);
    }

    public class SubAccountService : ISubAccountService
    {
        private readonly IDynamicRepository dynamicRepository;

        public SubAccountService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<Result> AddSubAccount(AddSubAccountModel subAccountModel)
        {
            var command = new AddSubAccountCriteria
            {
                CustId = subAccountModel.CustId,
                UserId = subAccountModel.CustId,
                Username = subAccountModel.Username,
                UserPwd = subAccountModel.Password,
                Firstname = subAccountModel.Firstname,
                Lastname = subAccountModel.Lastname,
                PageAccess = subAccountModel.PageAccess,
                LoginIP = subAccountModel.IP,
                Language = subAccountModel.Language
            };

            await dynamicRepository.ExecuteAsync(command);

            return new Result(command.ErrCode, command.ErrMsg, command.SubAccId.ToString());
        }

        public async Task<SubAccountModel> GetSubAccount(int custId, int subAccId)
        {
            var result = await dynamicRepository.GetAsync<SubAccountModel>(new SubAccountCriteria { CustId = custId, SubAccId = subAccId });
            return result ?? new SubAccountModel();
        }

        public async Task<IEnumerable<SubAccountModel>> GetSubAccounts(int custId)
        {
            var result = await dynamicRepository.FetchAsync<SubAccountModel>(new SubAccountListCriteria { CustId = custId });
            return result ?? Enumerable.Empty<SubAccountModel>();
        }

        public async Task<Result> UpdateSubAccountInfo(UpdateSubAccountInfoModel subAccountModel)
        {
            var command = new UpdateSubAccountInfoCriteria
            {
                Closed = subAccountModel.Closed,
                CustId = subAccountModel.CustId,
                UserId = subAccountModel.CustId,
                Firstname = subAccountModel.Firstname,
                Lastname = subAccountModel.Lastname,
                Language = subAccountModel.Language,
                LoginIP = subAccountModel.IP,
                SubAccId = subAccountModel.SubAccId
            };

            await dynamicRepository.ExecuteAsync(command);

            return new Result(command.ErrCode, command.ErrMsg);
        }

        public async Task<Result> UpdateSubAccountPageAccess(UpdateSubAccountPageAccessModel subAccountModel)
        {
            var command = new UpdateSubAccountPageAccessCriteria
            {
                CustId = subAccountModel.CustId,
                Language = subAccountModel.Language,
                LoginIP = subAccountModel.IP,
                SubAccId = subAccountModel.SubAccId,
                UpdPageAccess = subAccountModel.PageAccess,
                UserId = subAccountModel.CustId
            };

            await dynamicRepository.ExecuteAsync(command);

            return new Result(command.ErrCode, command.ErrMsg);
        }
    }
}