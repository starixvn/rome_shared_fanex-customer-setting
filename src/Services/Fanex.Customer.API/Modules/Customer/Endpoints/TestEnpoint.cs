using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Customer.Endpoints;

public class TestEnpoint : IEndpoint<IResult, string, string, string>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Test", (string str1, string str2, string str3)
            => HandleAsync(str1, str2, str3));
        app.MapGet("/api/{str1}/{str2}/{str3}/Test", (string str1, string str2, string str3)
            => HandleAsync(str1, str2, str3));
    }

    public async Task<IResult> HandleAsync(string str1, string str2, string str3)
    {
        return Results.Ok($"str1: {str1}; str2: {str2}; str3: {str3}");
    }
}