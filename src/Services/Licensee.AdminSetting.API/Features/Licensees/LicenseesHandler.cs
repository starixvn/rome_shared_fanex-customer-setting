using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Licensees.Criteria;
using Licensee.AdminSetting.API.Features.Shared;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;
using System.Text.Json;

namespace Licensee.AdminSetting.API.Features.Licensees;

public interface ILicenseesHandler
{
    Task<Result> CreateLicensee(CreateLicenseeRequest request);
}

public class LicenseesHandler(IDynamicRepository repository, ISiteDBHandler siteDBHandler) : ILicenseesHandler
{
    private readonly IDynamicRepository repository = repository;
    private readonly ISiteDBHandler siteDBHandler = siteDBHandler;

    public async Task<Result> CreateLicensee(CreateLicenseeRequest request)
    {
        var customerSiteDb = await siteDBHandler.GetCustomerSiteDB(request.SiteDBGroupId);
        var validationResult = await ValidateCreateLicensee(request, customerSiteDb.GroupId);

        if (validationResult.IsNotOK)
        {
            return validationResult;
        }

        var registerResult = await RegisterCustomers(request, validationResult.RegisterUsernames);
        if (registerResult.IsNotOK)
        {
            return registerResult;
        }

        return await CreateLicensee(request, registerResult.Customers, customerSiteDb.GroupId);
    }

    private async Task<ValidateCreateLicenseeResult> ValidateCreateLicensee(CreateLicenseeRequest request, int custDBGroupId)
    {
        var command = new CreateLicenseeValidateCommand(
            custDBGroupId,
            JsonSerializer.Serialize(request.ProductBetSetting),
            JsonSerializer.Serialize(request.ProductPT),
            JsonSerializer.Serialize(request.ProductStakeLevel),
            request.SiteName);

        await repository.ExecuteAsync(command);

        return new ValidateCreateLicenseeResult(command.ErrCode, command.ErrMsg, command.RegisterUsernames);
    }

    private async Task<RegisterCustomersResult> RegisterCustomers(CreateLicenseeRequest request, string registerUsernames)
    {
        var command = new RegisterCustomersCommand(registerUsernames, request.SiteName, request.IsChinaView, request.SpecialPlatform, 1, request.SiteDBGroupId);
        await repository.ExecuteAsync(command);
        return new RegisterCustomersResult(command.ErrCode, command.ErrMsg, command.SiteId, command.Customers);
    }

    private async Task<Result> CreateLicensee(CreateLicenseeRequest request, string customers, int custDBGroupId)
    {
        var command = new CreateLicenseeCommand(
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
            request.SpecialPlatform);

        await repository.ExecuteAsync(command);

        return new Result(command.ErrCode, command.ErrMsg);
    }
}