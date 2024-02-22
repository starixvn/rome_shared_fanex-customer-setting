using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Shared.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class LanguageUsageEndpoint : IEndpoint<Result, UsageLanguageRequest, ILanguageService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/usage-language", (UsageLanguageRequest request, ILanguageService nicknameService)
            => HandleAsync(request, nicknameService))
            .Produces<Result>()
            .WithTags("Customer Settings");
    }

    public async Task<Result> HandleAsync(UsageLanguageRequest request1, ILanguageService request2)
    {
        return await request2.UpdateUsageLanguage(request1);
    }
}