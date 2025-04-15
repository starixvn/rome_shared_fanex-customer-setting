using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Customer.API.Modules.Customer.Requests;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class SearchSuggestDirectMembersEndpoint : IEndpoint<IEnumerable<SuggestDirectMember>, int, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/{uplineCustId}/suggest-direct-members/{searchUserName}", (int uplineCustId, string searchUserName, ICustomerService customerService)
            => HandleAsync(uplineCustId, searchUserName, customerService))
            .Produces<SuggestDirectMember>()
            .WithTags("Direct Member");
    }

    public async Task<IEnumerable<SuggestDirectMember>> HandleAsync(int uplineCustId, string searchUserName, ICustomerService customerService)
    {
        if (uplineCustId > 0 && !string.IsNullOrWhiteSpace(searchUserName))
        {
            return await customerService.SearchSuggestionDirectMember(uplineCustId, searchUserName);
        }

        return null;
    }
}