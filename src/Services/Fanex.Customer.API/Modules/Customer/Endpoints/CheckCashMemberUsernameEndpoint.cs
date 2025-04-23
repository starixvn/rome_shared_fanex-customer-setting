using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class CheckCashMemberUsernameEndpoint : IEndpoint<int, int, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{uplineCustId}/check-cash-member/{userName}", (int uplineCustId, string userName, ICustomerService customerService)
            => HandleAsync(uplineCustId, userName, customerService))
            .Produces<int>()
            .WithTags("Cash Member");
    }

    public async Task<int> HandleAsync(int uplineCustId, string userName, ICustomerService customerService)
    {
        if (uplineCustId > 0 && !string.IsNullOrWhiteSpace(userName))
        {
            return await customerService.CheckCashMember(uplineCustId, userName);
        }

        return 0;
    }
}