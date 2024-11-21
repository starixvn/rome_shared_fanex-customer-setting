using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Modules.QueryCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.QueryCustomer.EndPoints;

public class QueryCustomerSettingsEndpoint : IEndpoint<IResult, QuerySuperRequest, IQueryCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/globalsetting", ([AsParameters] QuerySuperRequest request, IQueryCustomerService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingModel>().WithTags("Customer Settings");

        app.MapGet("/api/customer", ([AsParameters] QueryRequest request, IQueryCustomerService service)
            => HandleDownlineAsync(request, service))
            .Produces<CustomerSettingModel>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(QuerySuperRequest request, IQueryCustomerService service)
    {
        return Results.Ok(await service.QuerySuper(request));
    }

    public async Task<IResult> HandleDownlineAsync(QueryRequest request, IQueryCustomerService service)
    {
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }

        if (!request.UplineRoleId.HasValue)
        {
            request.UplineRoleId = (byte?)(request.RoleId + 1);
        }

        request.IsDirectMember = request.IsDirectMember.GetValueOrDefault() ? true : null;

        return Results.Ok(await service.Query(request));
    }
}