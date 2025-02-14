using System.Globalization;
using System.Text.RegularExpressions;
using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Customer.API.Modules.SecurityCode.Criteria;
using Fanex.Customer.API.Modules.SecurityCode.Params;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using TelegramBot.Client.Services;
using SecurityCodeResource = Fanex.Customer.API.Modules.AppResources.Security;

namespace Fanex.Customer.API.Modules.SecurityCode;

public interface ISecurityCodeService
{
    string GenerateSalt(int custId);

    Task<Result> VerifySecurityCode(VerifySecurityCodeRequest param);

    Task<Result> CheckSecurityCode(CheckSecurityCodeRequest param);

    Task<string> GetSecurityCode(int custId, int subAccountId);

    Task<Result> ChangeMainAccountSecurityCode(ChangeMainAccountSecurityCodeParam param);

    Task<Result> ChangeSubAccountSecurityCode(ChangeSubAccountSecurityCodeParam param);

    Task<Result> ResetSubAccountSecurityCode(ResetSubAccountSecurityCodeParam param);

    string GenerateSecurityToken();

    Task<Result> UpdateSecurityCodeFunction(UpdateSecurityCodeFunctionParam param);
}

public class SecurityCodeService : BaseService, ISecurityCodeService
{
    private const string MD5 = "MD5";
    private const string SHA1 = "SHA1";
    private const string SHA256 = "SHA256";
    private const string SECURITYCODEPADDING = "1qaw3+{:P)";
    private const int AllowSecurityCodeLength = 6;
    private const string SecurityCodeRule = @"^([0-9])(?!\1+$)[0-9]+$";
    private const int Length = 6;
    private const int AccountClosed = 200;
    private const int AccountClosedStatus = 2;
    private const int WrongSecurityCode = 9009;

    private readonly IEncryption encryption;
    private readonly INotificationService notificationService;

    public SecurityCodeService(IDynamicRepository dynamicRepository,
        IEncryption encryption,
        INotificationService notificationService) : base(dynamicRepository)
    {
        this.encryption = encryption;
        this.notificationService = notificationService;
    }

    public string GenerateSalt(int custId)
        => Shared.Utils.Encryption.ComputeHash(MD5, custId.ToString(), SECURITYCODEPADDING);

    public async Task<Result> VerifySecurityCode(VerifySecurityCodeRequest param)
    {
        var actualSecurityCode = await GetSecurityCode(param.CustId, param.SubAccId).ConfigureAwait(false);
        var salt = GenerateSalt(param.IsSubAccount() ? param.SubAccId : param.CustId);
        var hashedSecurityCode = Shared.Utils.Encryption.ComputeHash(SHA1, param.SecurityCode, salt);
        var isCorrectSecurityCode = string.Equals(actualSecurityCode, hashedSecurityCode);
        return await UpdateSecurityCodeCounter(param, isCorrectSecurityCode);
    }

    public Task<string> GetSecurityCode(int custId, int subAccountId)
        => dynamicRepository.GetAsync<string>(subAccountId.Equals(0)
            ? new GetMainAccountSecurityCodeCriteria(custId)
            : new GetSubAccountSecurityCodeCriteria(custId, subAccountId));

    private async Task<Result> UpdateSecurityCodeCounter(VerifySecurityCodeRequest param, bool isCorrectSecurityCode)
    {
        var result = param.SubAccId.Equals(0)
                ? await ExecuteCommand(
                    new UpdateMainAccountSecurityCodeCounterCriteria
                    {
                        CustId = param.CustId,
                        CheckedSecurityCode = isCorrectSecurityCode,
                        IP = param.IP,
                        Language = param.Language
                    })
                : await ExecuteCommand(
                    new UpdateSubAccountSecurityCodeCounterCriteria
                    {
                        CustId = param.CustId,
                        SubaccId = param.SubAccId,
                        CheckedSecurityCode = isCorrectSecurityCode,
                        IP = param.IP,
                        Language = param.Language
                    });

        if (result.Code == AccountClosed)
        {
            result.Code = AccountClosedStatus; //// account is closed
        }
        else if (!result.IsSuccessful)
        {
            result.Code = WrongSecurityCode; //// wrong SC
            result.Message = SecurityCodeResource.alertInputWrongSecCode;
        }

        return result;
    }

    public async Task<Result> ChangeMainAccountSecurityCode(ChangeMainAccountSecurityCodeParam param)
    {
        var validateResult = await Validate(param.NewSecurityCode, param);
        if (!validateResult.IsSuccessful)
        {
            return validateResult;
        }

        var result = await ExecuteCommand(
             new UpdateMainAccountSecurityCodeCriteria
             {
                 CustId = param.CustId,
                 SubaccId = param.SubAccId,
                 UserId = param.UserId,
                 HashedSecurityCode = HashSecurityCode(param.CustId, param.NewSecurityCode, param.IsResetMember),
                 IP = param.IP,
                 Language = param.Language
             });

        if (result != null && result.IsSuccessful && !param.IsMemberLevel)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => notificationService.NotifyChangeSecurityCode(param.CustId, 0));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        return result;
    }

    public async Task<Result> ChangeSubAccountSecurityCode(ChangeSubAccountSecurityCodeParam param)
    {
        var validateResult = await Validate(param.NewSecurityCode, param);
        if (!validateResult.IsSuccessful)
        {
            return validateResult;
        }

        var command = new UpdateSubAccountSecurityCodeCriteria
        {
            CustId = param.CustId,
            SubaccId = param.SubAccId,
            HashedSecurityCode = HashSecurityCode(param.SubAccId, param.NewSecurityCode),
            IP = param.IP,
            Language = param.Language
        };

        var result = await ExecuteCommand(command);

        if (result != null && result.IsSuccessful && !param.IsMemberLevel)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => notificationService.NotifyChangeSecurityCode(param.CustId, param.SubAccId));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        return result;
    }

    public async Task<Result> ResetSubAccountSecurityCode(ResetSubAccountSecurityCodeParam param)
    {
        var validateResult = await Validate(param.NewSecurityCode);
        if (!validateResult.IsSuccessful)
        {
            return validateResult;
        }

        var command = new ResetSubAccountSecurityCodeCriteria
        {
            CustId = param.CustId,
            SubaccId = param.SubAccId,
            UserId = param.UserId,
            HashedSecurityCode = HashSecurityCode(param.SubAccId, param.NewSecurityCode),
            IP = param.IP,
            Language = param.Language
        };

        var result = await ExecuteCommand(command);

        if (result != null && result.IsSuccessful && !param.IsMemberLevel)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => notificationService.NotifyChangeSecurityCode(param.CustId, param.SubAccId));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        return result;
    }

    public string GenerateSecurityToken()
    {
        return this.encryption.RandomString(Length);
    }

    public Task<Result> UpdateSecurityCodeFunction(UpdateSecurityCodeFunctionParam param)
    {
        return ExecuteCommand(new UpdateSecurityCodeFunctionCriteria(param));
    }

    private async Task<Result> Validate(string securityCode, ChangeSubAccountSecurityCodeParam param = null)
    {
        if (string.IsNullOrWhiteSpace(securityCode))
        {
            return Result.CreateError(SecurityCodeResource.ResourceManager.GetString("alertSecCodeEmpty", new CultureInfo(LanguageUtils.ToLanguage(param.Language))));
        }

        if (!IsMatchedRules(securityCode))
        {
            return Result.CreateError(SecurityCodeResource.ResourceManager.GetString("lblSecCodeWrongRule", new CultureInfo(LanguageUtils.ToLanguage(param.Language))));
        }

        if (param != null && param.MustVerifyCurrentSecurityCode)
        {
            if (string.IsNullOrWhiteSpace(param.CurrentSecurityCode))
            {
                return Result.CreateError(SecurityCodeResource.ResourceManager.GetString("alertOldSecCodeEmpty", new CultureInfo(LanguageUtils.ToLanguage(param.Language))));
            }

            var verifyResult = await CheckSecurityCode(new CheckSecurityCodeRequest
            {
                CustId = param.CustId,
                SecurityCode = param.CurrentSecurityCode,
                IP = param.IP,
                Language = param.Language,
                Salt = param.Salt,
                SubAccId = param.SubAccId
            });

            if (!verifyResult.IsSuccessful)
            {
                return verifyResult;
            }
        }

        return new Result();
    }

    private string HashSecurityCode(int custId, string securityCode, bool isResetMember = false)
    {
        //// because the Member site doesn't want to edit his code.
        //// so that we must use the CfsEnCode method.
        if (isResetMember)
        {
            return encryption.CfsEnCode(securityCode);
        }

        var salt = GenerateSalt(custId);

        return encryption.ComputeHash(SHA1, securityCode, salt);
    }

    private static bool IsMatchedRules(string securityCode)
    {
        int securityCodeLength = securityCode == null ? 0 : securityCode.Trim().Length;
        if (securityCodeLength != AllowSecurityCodeLength)
        {
            return false;
        }

        Regex regex = new(SecurityCodeRule);
        //// check digits and all of them are not the same
        bool isMatched = regex.IsMatch(securityCode);
        if (isMatched)
        {
            //// check 'Forward Run' and 'Backward Run'
            bool isForwardRun = true, isBackwardRun = true;
            int forward = 0, backward = 0;
            forward = backward = int.Parse(securityCode[0].ToString());
            for (int i = 1; i < securityCodeLength; i++)
            {
                int current = int.Parse(securityCode[i].ToString());
                if (++forward != current)
                {
                    isForwardRun = false;
                }

                if (--backward != current)
                {
                    isBackwardRun = false;
                }
            }

            isMatched = !(isForwardRun || isBackwardRun);
        }

        return isMatched;
    }

    public async Task<Result> CheckSecurityCode(CheckSecurityCodeRequest param)
    {
        var actualSecurityCode = await GetSecurityCode(param.CustId, param.SubAccId).ConfigureAwait(false);
        var hashedSecurityCode = Shared.Utils.Encryption.ComputeHash(SHA256, actualSecurityCode, param.Salt);
        var isCorrectSecurityCode = string.Equals(param.SecurityCode, hashedSecurityCode);
        return await UpdateSecurityCodeCounter(param, isCorrectSecurityCode);
    }
}