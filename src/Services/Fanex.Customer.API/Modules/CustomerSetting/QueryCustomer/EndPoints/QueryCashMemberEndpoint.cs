using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.QueryCustomer.Requests;
using Fanex.Customer.API.Modules.QueryCustomer.Services;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.QueryCustomer.EndPoints;

public class QueryCashMemberEndpoint : IEndpoint<IResult, QueryCashMemberRequest, IQueryCashMemberService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/cash-member", ([AsParameters] QueryCashMemberRequest request, IQueryCashMemberService service)
            => HandleAsync(request, service))
            .Produces<CustomerSettingModel>().WithTags("Customer Settings");
    }

    public async Task<IResult> HandleAsync(QueryCashMemberRequest request, IQueryCashMemberService service)
    {
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            request.Language = "en-US";
        }

        return Results.Ok(await service.Query(request));
    }
}