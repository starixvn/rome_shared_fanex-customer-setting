using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints.CashService;

public class GetCountryCodesEndpoint : IEndpoint<IResult, string, ICustomerService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/cash-service/country-codes", (string site, ICustomerService customerService) =>
            HandleAsync(site, customerService))
            .Produces<IEnumerable<string>>()
            .WithTags("Cash Service")
            .WithSummary("Get available country codes for cash service")
            .WithDescription("Retrieves a list of country codes available for the specified site");
    }

    public async Task<IResult> HandleAsync(string site, ICustomerService customerService)
    {
        if (string.IsNullOrWhiteSpace(site))
        {
            return Results.BadRequest("Site parameter is required");
        }

        var countryCodes = await customerService.GetCountryCodes(site);
        return Results.Ok(countryCodes);
    }
}
