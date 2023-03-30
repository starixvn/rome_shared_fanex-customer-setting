using Fanex.Customer.API.Modules.Otp.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Otp.Endpoints;

public class VerifyOtpEndPoint : IEndpoint<IResult, CustomerOtp, IOtpService>
{
    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/otp/verify", (CustomerOtp customerOtp, IOtpService otpService)
            => HandleAsync(customerOtp, otpService))
            .Produces<IResult>();
    }

    public async Task<IResult> HandleAsync(CustomerOtp request, IOtpService otpService)
    {
        return Results.Ok(await otpService.VerifyOtp(request));
    }
}