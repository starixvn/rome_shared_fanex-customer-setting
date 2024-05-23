using Fanex.Data.Repository;
using MinimalApi.Endpoint;
using TelegramBot.Client.Services;

namespace Fanex.Customer.API.Modules.Otp;

public class CustomerOtpSettingEndPoint : IEndpoint<IResult, int, int, IDynamicRepository>
{
    private readonly ISettingService telegramService;

    public CustomerOtpSettingEndPoint(ISettingService telegramService)
    {
        this.telegramService = telegramService;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customer/otp/{custId}/{subAccId}", (int custId, int subAccId, IDynamicRepository dynamicRepository)
            => HandleAsync(custId, subAccId, dynamicRepository))
            .Produces<CustomerOtpSetting>()
            .WithTags(EndpointTags.OTP);
    }

    public async Task<IResult> HandleAsync(int custId, int subAccId, IDynamicRepository dynamicRepository)
    {
        var command = new CheckEnableOtpCommand
        {
            CustId = custId,
            SubAccId = subAccId
        };
        await dynamicRepository.ExecuteAsync(command);
        return Results.Ok(new CustomerOtpSetting(command.Enable, await telegramService.IsEnableOTP(custId, subAccId)));
    }
}