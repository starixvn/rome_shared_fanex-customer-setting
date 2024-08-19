using System.Text.Json;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Accounts;
using Licensee.AdminSetting.API.Features.Licensees.Criteria;
using Licensee.AdminSetting.API.Features.Shared;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using Licensee.AdminSetting.API.Features.UserLog;

namespace Licensee.AdminSetting.API.Features.Licensees
{
    public interface ICurrencyHandler
    {
        Task<Result> CreateCurrency(string site, CreateCurrencyRequest request);
    }

    public class CurrencyHandler(IDynamicRepository repository, ISiteDBHandler siteDBHandler, IUserLogHandler userLogHandler, IAccountsHandler accountsHandler)
        : BaseHandler(repository, siteDBHandler, userLogHandler, accountsHandler), ICurrencyHandler
    {
        public async Task<Result> CreateCurrency(string siteName, CreateCurrencyRequest request)
        {
            var siteDBConfig = await siteDBHandler.GetBySite(siteName);
            var validationResult = await ValidateRequest(siteName, request, siteDBConfig.CustDBGroupId);
            if (validationResult.IsNotOK)
            {
                return validationResult;
            }

            var failedCurrencies = new List<string>();
            var registerResult = await RegisterCustomers(
                validationResult.RegisterUsernames,
                siteName,
                request.ProductStakeLevel,
                request.IP,
                CreateMode.NewCurrency,
                siteDBConfig.CustDBGroupId);

            if (registerResult.result.IsNotOK)
            {
                return registerResult.result;
            }

            failedCurrencies.AddRange([.. registerResult.failedCurrencies]);

            var createResult = await CreateCurrency(request, registerResult.result.Customers, registerResult.result.SiteId, siteName, siteDBConfig);
            failedCurrencies.AddRange([.. createResult.failedCurrencies]);

            return createResult.result;
        }

        private async Task<(Result result, List<string> failedCurrencies)> CreateCurrency(
            CreateCurrencyRequest request,
            string customers,
            int siteId,
            string siteName,
            SiteDBConfig siteDBConfig)
        {
            var criteria = new CreateCurrencyCriteria(
                siteDBConfig.CustDBGroupId,
                customers,
                JsonSerializer.Serialize(request.ProductBetSetting),
                JsonSerializer.Serialize(request.ProductPT),
                JsonSerializer.Serialize(request.ProductStakeLevel),
                JsonSerializer.Serialize(request.ProductComm),
                request.Creator,
                request.IP,
                request.IsCreateSub,
                request.LicenseeType,
                siteId,
                siteName);

            var logParams = await repository.FetchAsync<LogParam>(criteria);
            var result = new Result(criteria.ErrCode, criteria.ErrMsg);

            if (result.IsOK && logParams.Any())
            {
                _ = Task.Run(() => WriteLogs(siteDBConfig.CustDBGroupId, logParams));
            }

            if (!string.IsNullOrWhiteSpace(criteria.InsBalExceptCustList))
            {
                _ = Task.Run(() => InsertBalanceException(criteria.InsBalExceptCustList, siteDBConfig.SiteDBGroupId));
            }

            _ = Task.Run(() => InsertCurrencyData(siteName, criteria.CurrencyList, criteria.PT_Sportsbook, request.IP));

            // if (!string.IsNullOrWhiteSpace(criteria.AgentList) && criteria.AgentList.Split(',').Length > 0)
            // {
            //     await CreateDummyMembers(siteDBConfig)
            // }

            if (!string.IsNullOrWhiteSpace(criteria.ListSuperRollBack))
            {
                await RollbackMultiple(criteria.ListSuperRollBack);
            }

            return (result, criteria.FailedCurrencies.Split(',').ToList());
        }

        private async Task<Result> InsertCurrencyData(string siteName, string currencyList, decimal PT_Sportsbook, string ip)
        {
            var insertCurrencyCommand = new InsertCurrencyCommand(siteName, currencyList, PT_Sportsbook, ip);
            await repository.ExecuteAsync(insertCurrencyCommand);

            if (insertCurrencyCommand.ErrCode != 0)
            {
                return new Result(insertCurrencyCommand.ErrCode, insertCurrencyCommand.ErrMsg);
            }

            var insertCompanyWinLimitCommand = new InsertCompanyWinLimitCommand(insertCurrencyCommand.LicenseeId);
            await repository.ExecuteAsync(insertCompanyWinLimitCommand);

            return new Result(insertCompanyWinLimitCommand.ErrCode, insertCompanyWinLimitCommand.ErrMsg);
        }
    }
}