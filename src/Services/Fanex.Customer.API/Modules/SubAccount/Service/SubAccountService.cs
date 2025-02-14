using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Customer.API.Modules.SubAccount.Command;
using Fanex.Customer.API.Modules.SubAccount.Model;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.SubAccount.Service
{
    public interface ISubAccountService
    {
        public Task<IEnumerable<SubAccountModel>> GetSubAccounts(int custId);

        public Task<SubAccountModel> GetSubAccount(int custId, int subAccId);

        public Task<Result> UpdateSubAccountInfo(int custId, UpdateSubAccountInfoModel subAccountModel);

        public Task<Result> UpdateSubAccountPageAccess(int custId, UpdateSubAccountPageAccessModel subAccountModel);

        public Task<Result> AddSubAccount(int custId, AddSubAccountModel subAccountModel);
    }

    public class SubAccountService : PasswordServiceBase, ISubAccountService
    {

        public SubAccountService(IDynamicRepository dynamicRepository, IEncryption encryption, IOptions<AppSettings> appSettings, HttpClient httpClient)
            : base(dynamicRepository, encryption, appSettings.Value, httpClient)
        {
        }

        public async Task<Result> AddSubAccount(int custId, AddSubAccountModel subAccountModel)
        {
            var register = new RegisterSubAccountCriteria(custId, subAccountModel.Username, subAccountModel.Language, subAccountModel.CustDBGroupID);

            await dynamicRepository.ExecuteAsync(register);

            if (register.ErrCode != 0)
            {
                return Result.CreateError(register.ErrMsg);
            }

            var command = new AddSubAccountCriteria
            {
                CustId = custId,
                UserId = subAccountModel.UserId,
                Username = subAccountModel.Username,
                UserPwd = await HashPassword(subAccountModel.Password),
                Firstname = subAccountModel.Firstname,
                Lastname = subAccountModel.Lastname,
                PageAccess = subAccountModel.PageAccess ?? string.Empty,
                LoginIP = subAccountModel.IP,
                Language = subAccountModel.Language,
                CustDBGroupID = subAccountModel.CustDBGroupID,
                SubAccId = register.SubAccId
            };

            await dynamicRepository.ExecuteAsync(command);

            if (command.SubAccId > 0 && command.ErrCode == 0)
            {
                await dynamicRepository.ExecuteAsync(new AddMsgAfterCreateNewSMACriteria(custId, register.SubAccId, subAccountModel.RoleId));
            }
            else
            {
                await dynamicRepository.ExecuteAsync(new RollbackSubAccountCriteria(custId, register.SubAccId));
            }

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

        public async Task<Result> UpdateSubAccountInfo(int custId, UpdateSubAccountInfoModel subAccountModel)
        {
            var command = new UpdateSubAccountInfoCriteria
            {
                Closed = subAccountModel.Closed,
                CustId = custId,
                UserId = subAccountModel.UserId,
                Firstname = subAccountModel.Firstname,
                Lastname = subAccountModel.Lastname,
                Language = subAccountModel.Language,
                LoginIP = subAccountModel.IP,
                SubAccId = subAccountModel.SubAccId
            };

            await dynamicRepository.ExecuteAsync(command);

            return new Result(command.ErrCode, command.ErrMsg);
        }

        public async Task<Result> UpdateSubAccountPageAccess(int custId, UpdateSubAccountPageAccessModel subAccountModel)
        {
            var command = new UpdateSubAccountPageAccessCriteria
            {
                CustId = custId,
                Language = subAccountModel.Language,
                LoginIP = subAccountModel.IP,
                SubAccId = subAccountModel.SubAccId,
                UpdPageAccess = subAccountModel.PageAccess ?? string.Empty,
                UserId = subAccountModel.UserId
            };

            await dynamicRepository.ExecuteAsync(command);

            if (command.ErrCode == 0)
            {
                await dynamicRepository.ExecuteAsync(new KickoutSubAccountCriteria(custId, subAccountModel.SubAccId));
            }

            return new Result(command.ErrCode, command.ErrMsg);
        }
    }
}