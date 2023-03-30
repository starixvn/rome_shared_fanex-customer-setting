using Fanex.Customer.API.Modules.Otp.Criteria;
using Fanex.Customer.API.Modules.Otp.Models;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using TelegramBot.Client.Services;
using SophosOtp = ONEbook.OTP.Services.OtpService;

namespace Fanex.Customer.API.Modules.Otp;

public interface IOtpService
{
    Task<CustomerOtpSetting> IsEnableOtp(int custId, int subAccId = 0);

    Task GenerateTelegramOtp(int custId, int subAccId);

    Task<CustomerOtp> GetCustomerOtp(int custId, int subAccId = 0);

    Task<Result> VerifyOtp(CustomerOtp customerOtp);
}

public class OtpService : IOtpService
{
    private const int TimePeriod = 30;
    private const int OtpSize = 6;
    private const int InvalidDataErrorCode = 1;
    private const string InvalidDataMessage = "Data is not valid";
    private const int DisabledAccountErrorCode = 200;
    private readonly IDynamicRepository dynamicRepository;
    private readonly ISettingService telegramService;
    private readonly IOTPService telegramOtpService;
    private readonly SophosOtp sophosOtp;

    public OtpService(IDynamicRepository dynamicRepository, ISettingService telegramService, IOTPService telegramOtpService, SophosOtp sophosOtp)
    {
        this.dynamicRepository = dynamicRepository;
        this.telegramService = telegramService;
        this.telegramOtpService = telegramOtpService;
        this.sophosOtp = sophosOtp;
    }

    public async Task<Result> VerifyOtp(CustomerOtp param)
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
            if (customerOtp.Enabled)
            {
                isCorrect = sophosOtp.VerifyOtp(customerOtp.SecretKey, param.Otp, TimePeriod, OtpSize);
            }
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

        var result = new Result(changeStatusCommand.ErrCode, isCorrect ? changeStatusCommand.ErrMsg : "OtpInvalidOrExpiredAndContactUpline");

        if (!isCorrect && result.Code == DisabledAccountErrorCode)
        {
            result.Message = "LockAccountOtpMessage";
        }

        return result;
    }

    public async Task<CustomerOtp> GetCustomerOtp(int custId, int subAccId = 0)
    {
        var customerOtp = await dynamicRepository.GetAsync<CustomerOtp>(new GetCustomerOtpCriteria { CustId = custId, SubAccId = subAccId });
        return customerOtp;
    }

    public async Task<CustomerOtpSetting> IsEnableOtp(int custId, int subAccId = 0)
    {
        var command = new CheckEnableOtpCommand
        {
            CustId = custId,
            SubAccId = subAccId
        };
        await dynamicRepository.ExecuteAsync(command);
        return new CustomerOtpSetting(command.Enable, await telegramService.IsEnableOTP(custId, subAccId));
    }

    public async Task GenerateTelegramOtp(int custId, int subAccId)
    {
        await telegramOtpService.SendOTP(custId, subAccId);
    }
}