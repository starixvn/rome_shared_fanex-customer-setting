using System.Globalization;
using Fanex.Customer.API.Modules.Otp.Commands;
using Fanex.Customer.API.Modules.Otp.Params;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using ONEbook.OTP.Services;
using CustomResult = Fanex.Customer.API.Shared.Models.Result;
using OtpSecurity = Fanex.Customer.API.Modules.AppResources.Security;

namespace Fanex.Customer.API.Modules.Otp
{
    public interface ICustomerOtpService
    {
        Task<CustomResult> SetUpOtp(CustomerOtp param);

        Task<CustomResult> RemoveSecretKey(RemoveSecretKeyParam param);

        Task<CustomResult> ChangeOtpStatus(ChangeOtpStatusParam param);

        CustomerOtp GenerateCustomerOtp(int custId, string username, int subAccId = 0);

        Task<CustomerOtp> GetCustomerOtp(int custId, int subAccId = 0);

        Task<bool> IsEnabledOtp(int custId, int subAccId = 0);

        Task<bool> IsDifferenceDevice(int custId, string deviceId, int subAccId = 0);
    }

    public class CustomerOtpService : ICustomerOtpService
    {
        private const int TimePeriod = 30;
        private const int OtpSize = 6;
        private const int SecretKeyLength = 20;
        private const int DisabledAccountErrorCode = 200;
        private const string Issuer = "MyAppName";
        private const string NotDetectedDevice = "N/A";
        private const int InvalidDataErrorCode = 1;
        private const string InvalidDataMessage = "Data is not valid";
        private const int DownlineIsDisabledErrorCode = 600;
        private const int DownlineDoesNotHaveRegisteredDeviceErrorCode = 100;
        private const int FailedToRemoveDownlineOtpErrorCode = 101;
        private const string CustIdCannotBeNegOrZero = "Cust Id cannot be negative or zero";

        private readonly IDynamicRepository dynamicRepository;
        private readonly OtpService otpService;
        private readonly IJsonHelper jsonHelper;
        private readonly Logging.ILogger logger;

        public CustomerOtpService(
            IDynamicRepository dynamicRepository,
            OtpService otpService,
            IJsonHelper jsonHelper,
            Logging.ILogger logger)
        {
            this.dynamicRepository = dynamicRepository;
            this.otpService = otpService;
            this.jsonHelper = jsonHelper;
            this.logger = logger;
        }

        public virtual async Task<CustomResult> ChangeOtpStatus(ChangeOtpStatusParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param), "Change Otp Status Parameters cannot be null");
            }

            var command = new UpdateCustomerOtpStatusCommand
            {
                CustId = param.CustId,
                SubAccId = param.SubAccId,
                Enable = param.IsEnable,
                IP = param.IP,
                Language = param.Language,
                UserId = param.ActorId,
                ErrCode = InvalidDataErrorCode,
                ErrMsg = InvalidDataMessage
            };

            await dynamicRepository.ExecuteAsync(command);

            return new CustomResult
            {
                Code = command.ErrCode,
                Message = command.ErrCode == 0 ? OtpSecurity.ResourceManager.GetString("UpdateOtpStatusSuccess", new CultureInfo(LanguageUtils.ToLanguage(param.Language))) : command.ErrMsg
            };
        }

        public CustomerOtp GenerateCustomerOtp(int custId, string username, int subAccId = 0)
        {

            var customerOtp = new CustomerOtp
            {
                SecretKey = otpService.GenerateSecretKey(SecretKeyLength),
                CustId = custId,
                SubAccId = subAccId,
                OtpUsername = username,
            };
            customerOtp.QRCode = otpService.GenerateQrCode(customerOtp.SecretKey, TimePeriod, OtpSize, Issuer, customerOtp.OtpUsername);

            return customerOtp;
        }

        public async Task<bool> IsEnabledOtp(int custId, int subAccId = 0)
        {

            var command = new CheckEnableOtpCommand
            {
                CustId = custId,
                SubAccId = subAccId
            };

            await dynamicRepository.ExecuteAsync(command);

            return command.Enable;
        }

        public async Task<CustomResult> SetUpOtp(CustomerOtp param)
        {
            var isCorrect = otpService.VerifyOtp(param.SecretKey, param.Otp, TimePeriod, OtpSize);

            if (!isCorrect)
            {
                return CustomResult.Create(OtpSecurity.ResourceManager.GetString("OtpIsInvalidOrExpired", new CultureInfo(LanguageUtils.ToLanguage(param.Language))));
            }

            var deviceInfoSerialized = jsonHelper.Serialize(param.Device.Info);
            var command = new InsertCustomerOtpCommand
            {
                CustId = param.CustId,
                DeviceId = param.Device.Id,
                DeviceInfo = deviceInfoSerialized,
                IP = param.Device.Info.IP,
                Language = param.Language,
                SecretKey = param.SecretKey,
                SubAccId = param.SubAccId,
                ErrCode = InvalidDataErrorCode,
                ErrMsg = InvalidDataMessage
            };

            await dynamicRepository.ExecuteAsync(command);

            return new CustomResult
            {
                Code = command.ErrCode,
                Message = command.ErrCode == 0 ? OtpSecurity.ResourceManager.GetString("AuthenticatorAppVerified", new CultureInfo(LanguageUtils.ToLanguage(param.Language))) : command.ErrMsg
            };
        }

        public virtual async Task<CustomerOtp> GetCustomerOtp(int custId, int subAccId = 0)
        {
            var customerOtpDto = await dynamicRepository.GetAsync<CustomerOtpDto>(
                new GetCustomerOtpCriteria
                {
                    CustId = custId,
                    SubAccId = subAccId
                });

            if (customerOtpDto == null
                || customerOtpDto.IsDelete)
            {
                return new CustomerOtp();
            }

            return new CustomerOtp
            {
                CustId = customerOtpDto.CustId,
                Enabled = customerOtpDto.Enabled,
                SecretKey = customerOtpDto.SecretKey,
                SubAccId = customerOtpDto.SubAccId
            };
        }

        public async Task<CustomResult> RemoveSecretKey(RemoveSecretKeyParam param)
        {
            var command = new DeleteCustOtpSecretKeyCommand
            {
                CustId = param.CustId,
                SubAccId = param.SubAccId,
                IP = param.IP,
                Language = param.Language,
                UserId = param.ActorId,
                ErrCode = InvalidDataErrorCode,
                ErrMsg = InvalidDataMessage,
                DeviceInfo = jsonHelper.Serialize(param.DeviceInfo)
            };

            await dynamicRepository.ExecuteAsync(command);

            var deleteResult = new CustomResult
            {
                Code = command.ErrCode,
                Message = GetRemovedOtpMessage(command.ErrCode, param.IsResetDownlineOrSubAccount, command.ErrMsg, LanguageUtils.ToLanguage(param.Language))
            };

            return deleteResult;
        }

        private static string GetRemovedOtpMessage(int code, bool isResetDownlineOrSubAccount, string errorMessage, string language)
        {
            if (isResetDownlineOrSubAccount)
            {
                return code switch
                {
                    DownlineIsDisabledErrorCode => OtpSecurity.ResourceManager.GetString("RemoveDisableOtpDownline", new CultureInfo(language)),
                    DownlineDoesNotHaveRegisteredDeviceErrorCode => OtpSecurity.ResourceManager.GetString("NoRegisterDeviceError", new CultureInfo(language)),
                    FailedToRemoveDownlineOtpErrorCode => OtpSecurity.ResourceManager.GetString("FailedToRemoveOtp", new CultureInfo(language)),
                    0 => OtpSecurity.ResourceManager.GetString("RemoveRegisteredDeviceSuccess", new CultureInfo(language)),
                    _ => errorMessage,
                };
            }

            return code == 0 ? OtpSecurity.ResourceManager.GetString("RemoveOtpSuccessMessage", new CultureInfo(language)) : errorMessage;
        }

        public async Task<bool> IsDifferenceDevice(int custId, string deviceId, int subAccId = 0)
        {
            if (deviceId == NotDetectedDevice)
            {
                await logger.InfoAsync("User device was not detected");
                return false;
            }

            var devices = await dynamicRepository.FetchAsync<Device>(
                new GetDevicesCriteria
                {
                    CustId = custId,
                    SubAccId = subAccId
                });

            var currentDevice = devices.FirstOrDefault(device => device.Id == deviceId);

            return currentDevice == null;
        }
    }
}