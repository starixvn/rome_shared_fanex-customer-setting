using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Accounts;
using Licensee.AdminSetting.API.Features.Licensees.Criteria;
using Licensee.AdminSetting.API.Features.Shared;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using Licensee.AdminSetting.API.Features.UserLog;
using System.Text.Json;

namespace Licensee.AdminSetting.API.Features.Licensees;

public interface ILicenseesHandler
{
    Task<Result> CreateLicensee(CreateLicenseeRequest request);
}

public class LicenseesHandler(
    IConfiguration configuration,
    IDynamicRepository repository,
    ISiteDBHandler siteDBHandler,
    IUserLogHandler userLogHandler,
    IAccountsHandler accountsHandler,
    Fanex.Logging.ILogger logger)
    : BaseHandler(configuration, repository, siteDBHandler, userLogHandler, accountsHandler, logger), ILicenseesHandler
{
    public async Task<Result> CreateLicensee(CreateLicenseeRequest request)
    {
        var siteDBConfig = await siteDBHandler.GetBySiteDBGroupId(request.SiteDBGroupId);
        var validationResult = await ValidateRequest(request.SiteName, request, siteDBConfig.CustDBGroupId);

        if (validationResult.IsNotOK)
        {
            return validationResult;
        }

        var failedCurrencies = new List<string>();
        var registerResult = await RegisterCustomers(
            validationResult.RegisterUsernames,
            request.SiteName,
            request.ProductStakeLevel,
            request.IP,
            CreateMode.NewLicensee,
            siteDBConfig.SiteDBGroupId,
            request.IsChinaView,
            request.SpecialPlatform);

        if (registerResult.result.IsNotOK)
        {
            return registerResult.result;
        }

        failedCurrencies.AddRange([.. registerResult.failedCurrencies]);

        _ = Task.Run(() => InsertBetRestrictionSite(registerResult.result.SiteId, siteDBConfig.CustDBGroupId));

        var createResult = await CreateLicensee(request, registerResult.result.Customers, registerResult.result.SiteId, siteDBConfig.CustDBGroupId);
        failedCurrencies.AddRange([.. createResult.failedCurrencies]);

        return createResult.result;
    }

    private async Task<Result> InsertBetRestrictionSite(int siteId, int custDBGroupId)
    {
        var command = new InsertBetRestrictionSiteCommand(custDBGroupId, siteId);
        await repository.ExecuteAsync(command);
        return new Result(command.ErrCode, command.ErrMsg);
    }

    private async Task<(Result result, List<string> failedCurrencies)> CreateLicensee(CreateLicenseeRequest request, string customers, int siteId, int custDBGroupId)
    {
        var criteria = new CreateLicenseeCriteria(
            custDBGroupId,
            customers,
            JsonSerializer.Serialize(request.ProductBetSetting),
            JsonSerializer.Serialize(request.ProductPT),
            JsonSerializer.Serialize(request.ProductStakeLevel),
            JsonSerializer.Serialize(request.ProductComm),
            request.Creator,
            request.IP,
            request.IsCreateSub,
            request.LicenseeType,
            request.IsChinaView,
            request.SpecialPlatform,
            siteId,
            request.SiteName,
            SettingStatus);

        var logParams = await repository.FetchAsync<LogParam>(criteria);

        if (!string.IsNullOrWhiteSpace(criteria.ErrExtraInfo))
        {
            await logger.ErrorAsync(criteria.ErrExtraInfo);
        }

        var result = new Result(criteria.ErrCode, criteria.ErrMsg);

        if (result.IsOK)
        {
            var writeLogsTask = WriteLogs(custDBGroupId, logParams);
            var insertLicenseeDataTask = InsertLicenseeData(criteria.SiteName, criteria.ProductList, criteria.LicenseeType);
            await Task.WhenAll(writeLogsTask, insertLicenseeDataTask);
        }

        if (!string.IsNullOrWhiteSpace(criteria.InsBalExceptCustList))
        {
            _ = Task.Run(() => InsertBalanceException(criteria.InsBalExceptCustList, request.SiteDBGroupId));
        }

        if (!string.IsNullOrWhiteSpace(criteria.ListSuperRollBack))
        {
            await RollbackMultiple(criteria.ListSuperRollBack);
        }

        if (!string.IsNullOrWhiteSpace(criteria.AgentList) && criteria.AgentList.Split(',').Length > 0)
        {
            _ = Task.Run(() => CreateDummyMembers(criteria.IP, criteria.AgentList.Split(',').Select(x => Convert.ToInt32(x)).ToArray()));
        }

        return (result, criteria.FailedCurrencies?.Split(',').ToList() ?? []);
    }

    private async Task<Result> InsertLicenseeData(string siteName, string productList, string licenseeType)
    {
        var command = new InsertLicenseeDataCommand(siteName, productList, licenseeType);
        await repository.ExecuteAsync(command);
        var result = new Result(command.ErrCode, command.ErrMsg);

        if (result.IsOK)
        {
            return await InsertCompanyWinLimit(command.LicenseeId);
        }

        return result;
    }
}