using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Otp.Endpoints;

public class GenerateTelegramOtpEndPoint : IEndpoint<IResult, GenerateTelegramOtpRequest, IOtpService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/otp/telegram/generate", (GenerateTelegramOtpRequest request, IOtpService otpService)
            => HandleAsync(request, otpService))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(GenerateTelegramOtpRequest request, IOtpService otpService)
    {
        if (request.IsValid())
        {
            await otpService.GenerateTelegramOtp(request.CustId, request.SubAccId);
            return Results.Ok();
        }

        return Results.BadRequest();
    }
}