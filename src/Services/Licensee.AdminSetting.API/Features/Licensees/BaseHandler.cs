using System.Text.Json;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Accounts;
using Licensee.AdminSetting.API.Features.Licensees.Criteria;
using Licensee.AdminSetting.API.Features.Shared;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using Licensee.AdminSetting.API.Features.UserLog;

namespace Licensee.AdminSetting.API.Features.Licensees
{
    public abstract class BaseHandler(
        IConfiguration configuration,
        IDynamicRepository repository,
        ISiteDBHandler siteDBHandler,
        IUserLogHandler userLogHandler,
        Fanex.Logging.ILogger logger)
    {
        protected readonly IConfiguration configuration = configuration;
        protected readonly IDynamicRepository repository = repository;
        protected readonly ISiteDBHandler siteDBHandler = siteDBHandler;
        protected readonly IUserLogHandler userLogHandler = userLogHandler;
        protected readonly Fanex.Logging.ILogger logger = logger;

        protected byte SettingStatus = configuration.GetValue<byte>("SettingStatus", 0);

        protected async Task<ValidateCreateLicenseeResult> ValidateRequest(string siteName, byte mode, CreateCurrencyRequest request, int custDBGroupId)
        {
            var command = new SiteBuilderValidateCommand(
                custDBGroupId,
                JsonSerializer.Serialize(request.ProductBetSetting),
                JsonSerializer.Serialize(request.ProductPT),
                siteName,
                mode,
                SettingStatus,
                request.IsCreateSub);

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

            if (!string.IsNullOrWhiteSpace(command.ErrExtraInfo))
            {
                await logger.ErrorAsync(command.ErrExtraInfo);
            }

            return (new RegisterCustomersResult(command.ErrCode, command.ErrMsg, command.SiteId, command.Customers), command.FailedCurrencies?.Split(',')?.ToList() ?? []);
        }

        protected Task WriteLogs(int custDBGroupId, IEnumerable<LogParam> logParams)
        {
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = 3
            };
            return Parallel.ForEachAsync(logParams, parallelOptions, async (param, _) => await Task.Run(() => userLogHandler.WriteLog(custDBGroupId, param), _));
        }

        protected Task<int> InsertBalanceException(string insBalExceptCustList, int siteDBGroupId)
            => repository.ExecuteAsync(new InsertBalanceExceptionCommand(insBalExceptCustList, siteDBGroupId));

        protected Task<int> RollbackMultiple(string custIds)
            => repository.ExecuteAsync(new RollbackCreateLicenseeCommand(custIds));

        protected async Task<Result> InsertCompanyWinLimit(int licenseeId)
        {
            var command = new InsertCompanyWinLimitCommand(licenseeId);
            await repository.ExecuteAsync(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }
    }
}