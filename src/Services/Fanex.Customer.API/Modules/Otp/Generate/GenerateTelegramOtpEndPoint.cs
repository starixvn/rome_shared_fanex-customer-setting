using MinimalApi.Endpoint;
using TelegramBot.Client.Services;

namespace Fanex.Customer.API.Modules.Otp;

public class GenerateTelegramOtpEndPoint : IEndpoint<IResult, GenerateTelegramOtpRequest, IOTPService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/otp/telegram/generate", (GenerateTelegramOtpRequest request, IOTPService telegramOtpService)
            => HandleAsync(request, telegramOtpService))
            .Produces<IResult>()
            .WithTags(EndpointTags.OTP);
    }

    public async Task<IResult> HandleAsync(GenerateTelegramOtpRequest request, IOTPService telegramOtpService)
    {
        if (request.IsValid())
        {
            await telegramOtpService.SendOTP(request.CustId, request.SubAccId);
            return Results.Ok();
        }

        return Results.BadRequest();
    }
}