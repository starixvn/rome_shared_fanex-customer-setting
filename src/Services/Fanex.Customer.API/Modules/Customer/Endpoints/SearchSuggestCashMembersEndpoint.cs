using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class SearchSuggestCashMembersEndpoint : IEndpoint<IEnumerable<SuggestCashMember>, int, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{uplineCustId}/suggest-cash-members/{searchUserName}", (int uplineCustId, string searchUserName, ICustomerService customerService)
            => HandleAsync(uplineCustId, searchUserName, customerService))
            .Produces<SuggestCashMember>()
            .WithTags("Cash Service");
    }

    public async Task<IEnumerable<SuggestCashMember>> HandleAsync(int uplineCustId, string searchUserName, ICustomerService customerService)
    {
        if (uplineCustId > 0 && !string.IsNullOrWhiteSpace(searchUserName))
        {
            return await customerService.SearchSuggestionCashMember(uplineCustId, searchUserName);
        }

        return null;
    }
}