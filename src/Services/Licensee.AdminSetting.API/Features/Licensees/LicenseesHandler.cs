using Fanex.Data.Repository;
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

public class LicenseesHandler(IDynamicRepository repository, ISiteDBHandler siteDBHandler, IUserLogHandler userLogHandler) : ILicenseesHandler
{
    private readonly IDynamicRepository repository = repository;
    private readonly ISiteDBHandler siteDBHandler = siteDBHandler;
    private readonly IUserLogHandler userLogHandler = userLogHandler;

    public async Task<Result> CreateLicensee(CreateLicenseeRequest request)
    {
        var customerSiteDb = await siteDBHandler.GetCustomerSiteDB(request.SiteDBGroupId);
        var validationResult = await ValidateCreateLicensee(request, customerSiteDb.GroupId);

        if (validationResult.IsNotOK)
        {
            return validationResult;
        }

        var failedCurrencies = new List<string>();
        var registerResult = await RegisterCustomers(request, validationResult.RegisterUsernames);
        if (registerResult.result.IsNotOK)
        {
            return registerResult.result;
        }

        failedCurrencies.AddRange([.. registerResult.failedCurrencies]);

        var insertBetRestrictionSiteResult = await InsertBetRestrictionSite(registerResult.result.SiteId, customerSiteDb.GroupId);
        if (insertBetRestrictionSiteResult.IsNotOK)
        {
            return insertBetRestrictionSiteResult;
        }

        var createResult = await CreateLicensee(request, registerResult.result.Customers, registerResult.result.SiteId, customerSiteDb.GroupId);

        return createResult.result;
    }

    private async Task<ValidateCreateLicenseeResult> ValidateCreateLicensee(CreateLicenseeRequest request, int custDBGroupId)
    {
        var command = new CreateLicenseeValidateCommand(
            custDBGroupId,
            JsonSerializer.Serialize(request.ProductBetSetting),
            JsonSerializer.Serialize(request.ProductPT),
            JsonSerializer.Serialize(request.ProductStakeLevel),
            request.SiteName,
            CreateMode.NewLicensee);

        await repository.ExecuteAsync(command);

        return new ValidateCreateLicenseeResult(command.ErrCode, command.ErrMsg, command.RegisterUsernames);
    }

    private async Task<(RegisterCustomersResult result, List<string> failedCurrencies)> RegisterCustomers(CreateLicenseeRequest request, string registerUsernames)
    {
        var command = new RegisterCustomersCommand(
            registerUsernames,
            request.SiteName,
            request.IsChinaView,
            request.SpecialPlatform,
            JsonSerializer.Serialize(request.ProductStakeLevel),
            request.IP,
            CreateMode.NewLicensee,
            request.SiteDBGroupId);

        await repository.ExecuteAsync(command);

        return (new RegisterCustomersResult(command.ErrCode, command.ErrMsg, command.SiteId, command.Customers), command.FailedCurrencies?.Split(',')?.ToList());
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
            request.Creator,
            request.IP,
            request.IsCreateSub,
            request.LicenseeType,
            request.IsChinaView,
            request.SiteDBGroupId,
            request.SpecialPlatform,
            siteId);

        var logParams = await repository.FetchAsync<LogParam>(criteria);

        var result = new Result(criteria.ErrCode, criteria.ErrMsg);

        if (result.IsOK && logParams.Any())
        {
            _ = Task.Run(() => WriteCreateLicenseeLogs(custDBGroupId, logParams));
        }

        if (!string.IsNullOrWhiteSpace(criteria.InsBalExceptCustList))
        {
            _ = Task.Run(() => InsertBalanceException(criteria.InsBalExceptCustList));
        }

        if (!string.IsNullOrWhiteSpace(criteria.ListSuperRollBack))
        {
            await Rollback(criteria.ListSuperRollBack);
        }

        return (result, criteria.FailedCurrencies.Split(',').ToList());
    }

    private ParallelLoopResult WriteCreateLicenseeLogs(int custDBGroupId, IEnumerable<LogParam> logParams)
        => Parallel.ForEach(logParams, param => userLogHandler.WriteLog(custDBGroupId, param));

    private Task InsertBalanceException(string insBalExceptCustList)
        => repository.ExecuteAsync(new InsertBalanceExceptionCommand(insBalExceptCustList));

    private Task Rollback(string custIds)
        => repository.ExecuteAsync(new RollbackCreateLicenseeCommand(custIds));
}