using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class CheckExistDirectMemberUsernameEndpoint : IEndpoint<int, int, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{uplineCustId}/check-exist-direct-member/{userName}", (int uplineCustId, string userName, ICustomerService customerService)
            => HandleAsync(uplineCustId, userName, customerService))
            .Produces<int>()
            .WithTags("Direct Member");
    }

    public async Task<int> HandleAsync(int uplineCustId, string userName,  ICustomerService customerService)
    {
        if (uplineCustId > 0 && !string.IsNullOrWhiteSpace(userName))
        {
            return await customerService.CheckExistDirectMember(uplineCustId, userName);
        }

        return 0;
    }
}