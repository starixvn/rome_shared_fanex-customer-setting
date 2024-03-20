using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.CreateCustomer.EndPoints;

public class CurrencyEndpoint : IEndpoint<IResult, ICurrencyService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/currencies", (ICurrencyService service)
            => HandleAsync(service))
            .Produces<Currency>().WithTags("Create Customer Full Mode");
    }

    public async Task<IResult> HandleAsync(ICurrencyService service)
    {
        return Results.Ok(await service.GetCreditCurrencies());
    }
}