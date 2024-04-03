using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class QueryCustomerSettingsEndpoint : IEndpoint<IResult, QuerySuperRequest, IQueryCustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/globalsetting", ([AsParameters] QuerySuperRequest request, IQueryCustomerService service)
            => HandleAsync(request, service))
            .Produces<CustomerSetting>().WithTags("Customer Settings");

        app.MapGet("/api/customer", ([AsParameters] QueryRequest request, IQueryCustomerService service)
            => HandleDownlineAsync(request, service))
            .Produces<CustomerSetting>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(QuerySuperRequest request, IQueryCustomerService service)
    {
        Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(request));
        return Results.Ok(await service.QuerySuper(request));
    }

    public async Task<IResult> HandleDownlineAsync(QueryRequest request, IQueryCustomerService service)
    {
        Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(request));

        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }

        if (!request.UplineRoleId.HasValue)
        {
            request.UplineRoleId = (byte?)(request.RoleId + 1);
        }

        return Results.Ok(await service.Query(request));
    }
}