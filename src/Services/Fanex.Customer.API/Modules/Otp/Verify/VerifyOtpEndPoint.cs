using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using MinimalApi.Endpoint;
using TelegramBot.Client.Services;
using SophosOtp = ONEbook.OTP.Services.OtpService;
using OtpSecurityResource = Fanex.Customer.API.Modules.AppResources.Security;
using System.Globalization;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Modules.Otp.Commands;
using Newtonsoft.Json;

namespace Fanex.Customer.API.Modules.Otp;

public class VerifyOtpEndPoint : IEndpoint<IResult, VerifyOtpRequest, IDynamicRepository>
{
    private const int TimePeriod = 30;
    private const int OtpSize = 6;
    private const int InvalidDataErrorCode = 1;
    private const string InvalidDataMessage = "Data is not valid";
    private const int DisabledAccountErrorCode = 200;
    private readonly IDynamicRepository dynamicRepository;
    private readonly IOTPService telegramOtpService;
    private readonly SophosOtp sophosOtp;

    public VerifyOtpEndPoint(IDynamicRepository dynamicRepository, IOTPService telegramOtpService, SophosOtp sophosOtp)
    {
        this.dynamicRepository = dynamicRepository;
        this.telegramOtpService = telegramOtpService;
        this.sophosOtp = sophosOtp;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customer/otp/verify", (VerifyOtpRequest request, IDynamicRepository dynamicRepository)
            => HandleAsync(request, dynamicRepository))
            .Produces<IResult>()
            .WithTags(EndpointTags.OTP);
    }

    public async Task<IResult> HandleAsync(VerifyOtpRequest request, IDynamicRepository dynamicRepository)
    {
        return Results.Ok(await VerifyOtp(request));
    }

    private async Task<Result> VerifyOtp(VerifyOtpRequest param)
    {
        var isCorrect = false;

        if (param.IsEnabledTelegramOTP)
        {
            var verifyResult = await telegramOtpService.VerifyOTP(param.CustId, param.SubAccId, param.Otp);
            isCorrect = verifyResult.IsOk;
        }

        if (!isCorrect)
        {
            var customerOtp = await GetCustomerOtp(param.CustId, param.SubAccId);
            if (customerOtp != null && customerOtp.Enabled)
            {
                isCorrect = sophosOtp.VerifyOtp(customerOtp.SecretKey, param.Otp, TimePeriod, OtpSize);                
            }
        }

        if (isCorrect)
        {
            var command = new InsertCustomerDeviceInfoCommand
            {
                CustId = param.CustId,
                DeviceId = param.Device.Id,
                DeviceInfo = JsonConvert.SerializeObject(param.Device.Info),
                IP = param.Device.Info.IP,
                Language = param.Language,
                SubAccId = param.SubAccId
            };

            await dynamicRepository.ExecuteAsync(command);
        }

        var changeStatusCommand = new UpdateCustomerStatusAfterVerifyOtpCommand
        {
            CheckedOtp = isCorrect,
            IP = param.IP,
            Language = param.Language,
            CustId = param.CustId,
            SubAccId = param.SubAccId,
            ErrCode = InvalidDataErrorCode,
            ErrMsg = InvalidDataMessage
        };

        await dynamicRepository.ExecuteAsync(changeStatusCommand);

        var result = new Result(changeStatusCommand.ErrCode, isCorrect ? changeStatusCommand.ErrMsg :
            OtpSecurityResource.ResourceManager.GetString("OtpInvalidOrExpiredAndContactUpline", new CultureInfo(LanguageUtils.ToLanguage(param.Language))));

        if (!isCorrect && result.Code == DisabledAccountErrorCode)
        {
            result.Message = OtpSecurityResource.ResourceManager.GetString("LockAccountOtpMessage", new CultureInfo(LanguageUtils.ToLanguage(param.Language)));
        }

        return result;
    }

    public async Task<CustomerOtp> GetCustomerOtp(int custId, int subAccId = 0)
    {
        var customerOtp = await dynamicRepository.GetAsync<CustomerOtp>(new GetCustomerOtpCriteria { CustId = custId, SubAccId = subAccId });
        return customerOtp;
    }
}