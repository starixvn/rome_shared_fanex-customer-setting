using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Licensees.Criteria;
using Licensee.AdminSetting.API.Features.Shared;
using System.Text.Json;

namespace Licensee.AdminSetting.API.Features.Licensees;

public interface ILicenseesHandler
{
    Task<Result> CreateLicensee(CreateLicenseeRequest request);
}

public class LicenseesHandler(IDynamicRepository repository) : ILicenseesHandler
{
    private readonly IDynamicRepository repository = repository;

    public Task<Result> CreateLicensee(CreateLicenseeRequest request)
    {
        throw new NotImplementedException();
    }

    private Task<CustomerSiteDB> GetCustomerSiteDB(int siteDBGroupId)
        => repository.GetAsync<CustomerSiteDB>(new GetCustomerSiteDBCriteria(siteDBGroupId));

    private async Task<ValidateCreateLicenseeResult> ValidateCreateLicensee(CreateLicenseeRequest request, int custDBGroupId)
    {
        var command = new CreateLicenseeValidateCommand(
            custDBGroupId,
            JsonSerializer.Serialize(request.ProductBetSetting),
            JsonSerializer.Serialize(request.ProductPT),
            JsonSerializer.Serialize(request.ProductStakeLevel),
            request.SiteName);

        await repository.ExecuteAsync(command);

        return new ValidateCreateLicenseeResult(command.ErrCode, command.ErrMsg, command.Customers);
    }

    // private async Task<Result> RegisterCustomers(CreateLicenseeRequest request, string customers)
}