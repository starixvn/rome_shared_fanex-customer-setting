using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Accounts.Criteria;
using Licensee.AdminSetting.API.Features.Shared;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Accounts
{
    public interface IAccountsHandler
    {
        Task<Result> CreateDummyMember(CreateDummyMemberRequest request);
    }

    public class AccountsHandler(IDynamicRepository repository, ISiteDBHandler siteDBHandler, Fanex.Logging.ILogger logger) : IAccountsHandler
    {
        private readonly IDynamicRepository repository = repository;
        private readonly ISiteDBHandler siteDBHandler = siteDBHandler;
        private readonly Fanex.Logging.ILogger logger = logger;

        public async Task<Result> CreateDummyMember(CreateDummyMemberRequest request)
        {
            var siteDBConfig = await siteDBHandler.GetByCustId(request.AgentId);
            var siteId = await siteDBHandler.GetSiteId(siteDBConfig.Site);

            var checkOddspreadAdjustmentResult = await CheckOddspreadAdjustment(siteId);
            if (checkOddspreadAdjustmentResult.IsNotOK)
            {
                return checkOddspreadAdjustmentResult;
            }

            return await CreateDummyMember(siteDBConfig, siteDBConfig.Site, request.AgentId, request.Creator, request.IP);
        }

        private async Task<Result> CheckOddspreadAdjustment(string siteId)
        {
            var command = new CheckOddsAdjCommand(siteId);
            await repository.ExecuteAsync(command);

            return new Result(command.ErrCode, command.ErrMsg);
        }

        private async Task<CreateDummyMemberResult> CreateDummyMember(SiteDBConfig siteDBConfig, string site, int agentId, int creator, string ip)
        {
            var generateResult = await GenerateDummyMember(siteDBConfig.CustDBGroupId, site, agentId);
            if (generateResult.IsNotOK)
            {
                return generateResult;
            }

            var createResult = await CreateMember(siteDBConfig, generateResult.CustId, generateResult.Username, agentId, creator, ip);
            if (createResult.IsOK)
            {
                return generateResult;
            }
            else
            {
                return new CreateDummyMemberResult(createResult.ErrorCode, createResult.ErrorMessage, 0, string.Empty);
            }
        }

        private async Task<CreateDummyMemberResult> GenerateDummyMember(int siteDBGroupId, string site, int agentId)
        {
            var command = new RegisterDummyMemberCommand(agentId, site, siteDBGroupId);
            await repository.ExecuteAsync(command);
            return new CreateDummyMemberResult(command.ErrCode, command.ErrMsg, command.CustId, command.Username);
        }

        private async Task<Result> CreateMember(SiteDBConfig siteDBConfig, int custId, string username, int agentId, int creator, string ip)
        {
            var command = new CreateDummyMemberCommand(siteDBConfig.CustDBGroupId, custId, username, creator, agentId, ip);
            await repository.ExecuteAsync(command);

            if (!string.IsNullOrWhiteSpace(command.ErrExtraInfo))
            {
                await logger.ErrorAsync(command.ErrExtraInfo);
            }

            var result = new Result(command.ErrCode, command.ErrMsg);

            if (result.IsOK)
            {
                await InsertBalanceException(siteDBConfig.SiteDBGroupId, custId, username);
            }
            else
            {
                await Rollback(custId);
            }

            return result;
        }

        private Task<int> InsertBalanceException(int siteDBGroupId, int custId, string username)
            => repository.ExecuteAsync(new InsertBalanceExceptionCommand(siteDBGroupId, custId, username));

        private Task<int> Rollback(int custId)
            => repository.ExecuteAsync(new RollbackCommand(custId));
    }
}