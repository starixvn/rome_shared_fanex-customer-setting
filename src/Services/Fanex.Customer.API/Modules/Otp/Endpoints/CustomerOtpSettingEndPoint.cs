using Fanex.Customer.API.Modules.Otp.Models;
using MinimalApi.Endpoint;

namespace Fanex.Customer.API.Modules.Otp.Endpoints;

public class CustomerOtpSettingEndPoint : IEndpoint<IResult, int, int>
{
    private readonly IOtpService otpService;

    public CustomerOtpSettingEndPoint(IOtpService otpService)
    {
        this.otpService = otpService;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/otp/{custId}/{subAccId}", (int custId, int subAccId)
            => HandleAsync(custId, subAccId))
            .Produces<CustomerOtpSetting>();
    }

    public async Task<IResult> HandleAsync(int custId, int subAccId)
    {
        return Results.Ok(await otpService.IsEnableOtp(custId, subAccId));
    }
}