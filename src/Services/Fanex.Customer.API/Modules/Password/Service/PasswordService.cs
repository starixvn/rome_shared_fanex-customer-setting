using System.Text.RegularExpressions;
using Fanex.Cryptography.Aes;
using Fanex.Customer.API.Modules.Password.Criteria;
using Fanex.Customer.API.Modules.Password.Model;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data.Repository;
using PasswordSecurity = Fanex.Customer.API.Modules.Password.Resources.Security;

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

    public class PasswordService : BaseService, IPasswordService
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
            ITelegramBotService telegramBotService)
            : base(dynamicRepository)
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
                false);

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var encodedPassword = encryption.CfsEnCode(rawNewPassword);
            var criteria = new UpdateMainAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                OldPwd = encryption.CfsEnCode(rawCurrentPassword),
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
                    Task.Run(() => telegramBotService.NotifyChangePassword(param.CustId, 0));
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
                false);

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var criteria = new UpdateSubAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                OldPwd = encryption.CfsEnCode(rawCurrentPassword),
                UserPwd = encryption.CfsEnCode(rawNewPassword),
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
                    Task.Run(() => telegramBotService.NotifyChangePassword(param.CustId, param.SubAccountId));
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
                param.IsResetMemberPassword);

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var encodedPassword = encryption.CfsEnCode(rawPassword);
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
            // TODO
            // if (netThrottlingService.IsBlacklist())
            // {
            //     return Result.CreateErrorResult("Too many requests");
            // }

            var rawNewPassword = aes.Decrypt(param.NewPassword, param.HashKey);
            var validateResult = Validate(
                true,
                string.Empty,
                rawNewPassword,
                new List<string>
                {
                    param.ActorName
                },
                false);

            if (!validateResult.IsSuccessful)
            {
                return validateResult;
            }

            var criteria = new ResetSubAccountPasswordCriteria
            {
                CustId = param.CustId,
                Language = param.LanguageKey,
                LoginIP = param.IP,
                UserPwd = encryption.CfsEnCode(rawNewPassword),
                SubAccId = param.SubAccountId,
                UserId = param.ActorId
            };

            var updateResult = await ExecuteCommand(criteria);

            if (updateResult.IsSuccessful)
            {
                // netThrottlingService.Throttle();

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
            }

            return updateResult;
        }

        private static Result Validate(
            bool isResetPassword,
            string currentPassword,
            string newPassword,
            IEnumerable<string> invalidPasswords,
            bool isResetMemberPassword)
        {
            if ((!isResetPassword && string.IsNullOrWhiteSpace(currentPassword))
                || string.IsNullOrWhiteSpace(newPassword))
            {
                return Result.CreateErrorResult(PasswordSecurity.NotAuthorized);
            }

            if (!IsMatchedRules(newPassword, isResetMemberPassword))
            {
                return Result.CreateErrorResult(PasswordSecurity.lblPasswordRequire);
            }

            if (!isResetPassword && string.Compare(newPassword, currentPassword, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Result.CreateErrorResult(PasswordSecurity.lblPasswordDOldOne); //// will update resource later
            }

            if (string.Compare(newPassword, "59D7!4$h", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(newPassword, "493abcDE", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Result.CreateErrorResult(PasswordSecurity.lblPasswordWrongRule);
            }

            foreach (var invalidPassword in invalidPasswords)
            {
                if (!string.IsNullOrWhiteSpace(invalidPassword)
                    && newPassword.Contains(invalidPassword, StringComparison.OrdinalIgnoreCase))
                {
                    return Result.CreateErrorResult(PasswordSecurity.lblPasswordDAccountName);
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