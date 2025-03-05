using System.Globalization;
using System.Text.RegularExpressions;
using Fanex.Cryptography.Aes;
using Fanex.Customer.API.Modules.Password.Criteria;
using Fanex.Customer.API.Modules.Password.Model;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using TelegramBot.Client.Extensions;
using PasswordSecurity = Fanex.Customer.API.Modules.AppResources.Security;

namespace Fanex.Customer.API.Modules.Password.Service
{
    public interface IPasswordService
    {
        Task<Result> ResetMainAccountPassword(ResetMainAccountPasswordParam param);

        Task<Result> ChangeMainAccountPassword(ChangeMainAccountPasswordParam param);

        Task<Result> ResetSubAccountPassword(ResetSubAccountPasswordParam param);

        Task<Result> ChangeSubAccountPassword(ChangeSubAccountPasswordParam param);

        Task<Result> SkipForceChangePassword(SkipForceChangePasswordParam param);

        Task<int> UpdateLastChangePasswordIndirectAccount(UpdateLastChangePasswordIndirectAccountParam param);
    }

    public class PasswordService : PasswordServiceBase, IPasswordService
    {
        private const int V = 3;
        private static readonly Regex PasswordRule1 = new Regex(@"^(?=.*\d).{8,100}$");
        private static readonly Regex PasswordRule2 = new Regex(@"^(?=.*[a-z]).{8,100}$");
        private static readonly Regex PasswordRule3 = new Regex(@"^(?=.*[A-Z]).{8,100}$");
        private static readonly Regex PasswordRule4 = new Regex(@"^(?=.*[\[\]\^\$\.\|\?\*\+\(\)\\~`\!@#%&\-_+={}'""<>:;,]).{8,100}$");
        private static readonly Regex MemberPasswordRule = new Regex(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$");
        private static readonly Regex NoWhiteSpace = new Regex(@"^\S*$");

        private readonly IEncryption encryption;
        private readonly IAesService aes;
        private readonly IKickoutCustomerService kickoutCustomerService;

        private readonly ITelegramBotService telegramBotService;

        public PasswordService(
            IEncryption encryption,
            IAesService aes,
            IKickoutCustomerService kickoutCustomerService,
            IDynamicRepository dynamicRepository,
            ITelegramBotService telegramBotService,
            IOptions<AppSettings> appSettings,
            HttpClient httpClient)
            : base(dynamicRepository, encryption, appSettings.Value, httpClient)
        {
            this.encryption = encryption;
            this.aes = aes;
            this.kickoutCustomerService = kickoutCustomerService;
            this.telegramBotService = telegramBotService;
        }

        public async Task<Result> ChangeMainAccountPassword(ChangeMainAccountPasswordParam param)
        {
            var rawCurrentPassword = aes.Decrypt(param.CurrentPassword, param.HashKey);
            var rawNewPassword = aes.Decrypt(param.NewPassword, param.HashKey);
            var validateResult = Validate(
                false,
                rawCurrentPassword,
                rawNewPassword,
                new List<string>
                {
                    param.ActorName,
                    param.NickName
                },
                false,
                LanguageUtils.ToLanguage(param.LanguageKey));

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var encodedPassword = await HashPassword(rawNewPassword);
            var criteria = new UpdateMainAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                OldPwd = await HashPassword(rawCurrentPassword),
                UserPwd = encodedPassword,
                UserId = param.ActorId
            };

            var updateResult = await ExecuteCommand(criteria);

            if (updateResult.IsSuccessful)
            {
                if (param.IsIndirectLoggedIn)
                {
                    await UpdateLastChangePasswordIndirectAccount(new UpdateLastChangePasswordIndirectAccountParam
                    {
                        CustId = param.IndirectCustId,
                        CustIdIndirect = param.CustId,
                        DownlineUsername = param.ActorName,
                        IP = param.IP,
                        Language = param.LanguageKey,
                        SubAccId = param.IndirectSubAccId,
                        SubAccIdIndirect = param.SubAccountId,
                        UplineUsername = param.IndirectUsername
                    });
                }

                if (!param.CustLevel.IsMemberLevel())
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(() => telegramBotService.NotifyChangePassword(param.CustId, 0));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }

            return updateResult;
        }

        public async Task<Result> ChangeSubAccountPassword(ChangeSubAccountPasswordParam param)
        {
            var rawCurrentPassword = aes.Decrypt(param.CurrentPassword, param.HashKey);
            var rawNewPassword = aes.Decrypt(param.NewPassword, param.HashKey);
            var validateResult = Validate(
                false,
                rawCurrentPassword,
                rawNewPassword,
                new List<string>
                {
                    param.SubUsername,
                    param.Nickname,
                    param.Username
                },
                false,
                LanguageUtils.ToLanguage(param.LanguageKey));

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var criteria = new UpdateSubAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                OldPwd = await HashPassword(rawCurrentPassword),
                UserPwd = await HashPassword(rawNewPassword),
                SubAccId = param.SubAccountId
            };

            var updateResult = await ExecuteCommand(criteria);

            if (updateResult.IsSuccessful)
            {
                if (param.IsIndirectLoggedIn)
                {
                    await UpdateLastChangePasswordIndirectAccount(new UpdateLastChangePasswordIndirectAccountParam
                    {
                        CustId = param.IndirectCustId,
                        CustIdIndirect = param.CustId,
                        DownlineUsername = param.ActorName,
                        IP = param.IP,
                        Language = param.LanguageKey,
                        SubAccId = param.IndirectSubAccId,
                        SubAccIdIndirect = param.SubAccountId,
                        UplineUsername = param.IndirectUsername
                    });
                }

                if (!param.CustLevel.IsMemberLevel())
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(() => telegramBotService.NotifyChangePassword(param.CustId, param.SubAccountId));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }

            return updateResult;
        }

        public async Task<Result> ResetMainAccountPassword(ResetMainAccountPasswordParam param)
        {
            var rawPassword = aes.Decrypt(param.NewPassword, param.HashKey);
            var validateResult = Validate(
                true,
                string.Empty,
                rawPassword,
                new List<string>
                {
                    param.ActorName
                },
                param.IsResetMemberPassword,
                LanguageUtils.ToLanguage(param.LanguageKey));

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var encodedPassword = await HashPassword(rawPassword);
            var criteria = new ResetMainAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                UserPwd = encodedPassword,
                SubAccId = param.SubAccountId,
                UserId = param.ActorId
            };

            var updateResult = await ExecuteCommand(criteria);

            if (updateResult.IsSuccessful)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await kickoutCustomerService.Kickout(new KickoutCustomerParam(
                        param.CustId,
                        param.CustLevel,
                        param.ServerId,
                        param.IP));

                    if (!param.CustLevel.IsMemberLevel())
                    {
                        telegramBotService.NotifyChangePassword(param.CustId, 0);
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            return updateResult;
        }

        public Task<Result> SkipForceChangePassword(SkipForceChangePasswordParam param)
        {
            var command = new SkipForceChangePasswordCommand
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                SubAccId = param.SubAccountId
            };

            return ExecuteCommand(command);
        }

        public async Task<Result> ResetSubAccountPassword(ResetSubAccountPasswordParam param)
        {
            var rawNewPassword = aes.Decrypt(param.NewPassword, param.HashKey);
            var validateResult = Validate(
                true,
                string.Empty,
                rawNewPassword,
                new List<string>
                {
                    param.ActorName
                },
                false,
                LanguageUtils.ToLanguage(param.LanguageKey));

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var criteria = new ResetSubAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                UserPwd = await HashPassword(rawNewPassword),
                SubAccId = param.SubAccountId,
                UserId = param.ActorId
            };

            var updateResult = await ExecuteCommand(criteria);

            if (updateResult.IsSuccessful)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    await kickoutCustomerService.Kickout(new KickoutCustomerParam(
                        param.CustId,
                        param.CustLevel,
                        param.ServerId,
                        param.IP,
                        param.SubAccountId));

                    if (!param.CustLevel.IsMemberLevel())
                    {
                        telegramBotService.NotifyChangePassword(param.CustId, param.SubAccountId);
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            return updateResult;
        }

        private static Result Validate(
            bool isResetPassword,
            string currentPassword,
            string newPassword,
            IEnumerable<string> invalidPasswords,
            bool isResetMemberPassword,
            string language)
        {
            if ((!isResetPassword && string.IsNullOrWhiteSpace(currentPassword))
                || string.IsNullOrWhiteSpace(newPassword))
            {
                return Result.CreateError(PasswordSecurity.ResourceManager.GetString("NotAuthorized", new CultureInfo(language)));
            }

            if (!IsMatchedRules(newPassword, isResetMemberPassword))
            {
                return Result.CreateError(PasswordSecurity.ResourceManager.GetString("lblPasswordRequire", new CultureInfo(language)));
            }

            if (!isResetPassword && string.Compare(newPassword, currentPassword, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Result.CreateError(PasswordSecurity.ResourceManager.GetString("lblPasswordDOldOne", new CultureInfo(language))); //// will update resource later
            }

            if (string.Compare(newPassword, "59D7!4$h", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(newPassword, "493abcDE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Result.CreateError(PasswordSecurity.ResourceManager.GetString("lblPasswordWrongRule", new CultureInfo(language)));
            }

            foreach (var invalidPassword in invalidPasswords)
            {
                if (!string.IsNullOrWhiteSpace(invalidPassword)
                    && newPassword.Contains(invalidPassword, StringComparison.OrdinalIgnoreCase))
                {
                    return Result.CreateError(PasswordSecurity.ResourceManager.GetString("lblPasswordDAccountName", new CultureInfo(language)));
                }
            }

            return new Result();
        }

        /// <summary>
        /// Password must be at least 8 characters without white space and contain at least 3 of the following:
        ///  • Uppercase letter [A-Z]
        ///  • Lowercase leader [a-z]
        ///  • Numeric [0-9]
        ///  • Special character (!,@,#,etc..)
        /// For example: 59D7!4$h
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static bool IsMatchedRules(string password, bool isResetMemberPassword)
        {
            if (isResetMemberPassword)
            {
                return MemberPasswordRule.Match(password).Success && NoWhiteSpace.Match(password).Success;
            }

            var r1 = PasswordRule1.Match(password).Success ? 1 : 0;
            var r2 = PasswordRule2.Match(password).Success ? 1 : 0;
            var r3 = PasswordRule3.Match(password).Success ? 1 : 0;
            var r4 = PasswordRule4.Match(password).Success ? 1 : 0;

            return (r1 + r2 + r3 + r4 >= V) && NoWhiteSpace.Match(password).Success;
        }

        public async Task<int> UpdateLastChangePasswordIndirectAccount(UpdateLastChangePasswordIndirectAccountParam param)
        {
            var criteria = new UpdateLastChangePasswordIndirectAccountCriteria
            {
                CustId = param.CustId,
                CustIdIndirect = param.CustIdIndirect,
                DownlineUsername = param.DownlineUsername,
                IP = param.IP,
                Language = param.Language,
                SubAccId = param.SubAccId,
                SubAccIdIndirect = param.SubAccIdIndirect,
                UplineUsername = param.UplineUsername
            };

            return await dynamicRepository.ExecuteAsync(criteria);
        }
    }
}