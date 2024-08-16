using System.Text.Json;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Licensees.Criteria;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using Licensee.AdminSetting.API.Features.UserLog;

namespace Licensee.AdminSetting.API.Features.Licensees
{
    public abstract class BaseHandler(IDynamicRepository repository, ISiteDBHandler siteDBHandler, IUserLogHandler userLogHandler)
    {
        protected readonly IDynamicRepository repository = repository;
        protected readonly ISiteDBHandler siteDBHandler = siteDBHandler;
        protected readonly IUserLogHandler userLogHandler = userLogHandler;

        protected async Task<ValidateCreateLicenseeResult> ValidateRequest(string siteName, CreateCurrencyRequest request, int custDBGroupId)
        {
            var command = new CreateLicenseeValidateCommand(
                custDBGroupId,
                JsonSerializer.Serialize(request.ProductBetSetting),
                JsonSerializer.Serialize(request.ProductPT),
                JsonSerializer.Serialize(request.ProductStakeLevel),
                JsonSerializer.Serialize(request.ProductComm),
                siteName,
                CreateMode.NewLicensee);

            await repository.ExecuteAsync(command);

            return new ValidateCreateLicenseeResult(command.ErrCode, command.ErrMsg, command.RegisterUsernames);
        }

        protected async Task<(RegisterCustomersResult result, List<string> failedCurrencies)> RegisterCustomers(
            string registerUsernames,
            string siteName,
            StakeLevel[] productStakeLevel,
            string ip,
            byte mode,
            int siteDBGroupId,
            bool isChinaView = false,
            byte specialPlatform = 0)
        {
            var command = new RegisterCustomersCommand(
                registerUsernames,
                siteName,
                isChinaView,
                specialPlatform,
                JsonSerializer.Serialize(productStakeLevel),
                ip,
                mode,
                siteDBGroupId);

            await repository.ExecuteAsync(command);

            return (new RegisterCustomersResult(command.ErrCode, command.ErrMsg, command.SiteId, command.Customers), command.FailedCurrencies?.Split(',')?.ToList());
        }

        protected ParallelLoopResult WriteLogs(int custDBGroupId, IEnumerable<LogParam> logParams)
            => Parallel.ForEach(logParams, param => userLogHandler.WriteLog(custDBGroupId, param));

        protected Task InsertBalanceException(string insBalExceptCustList, int siteDBGroupId)
            => repository.ExecuteAsync(new InsertBalanceExceptionCommand(insBalExceptCustList, siteDBGroupId));

        protected Task Rollback(string custIds)
            => repository.ExecuteAsync(new RollbackCreateLicenseeCommand(custIds));
    }
}