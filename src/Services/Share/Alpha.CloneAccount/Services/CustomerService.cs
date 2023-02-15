using System.Data;
using Alpha.CloneAccount.Configurations;
using Alpha.CloneAccount.Constants;
using Alpha.CloneAccount.Criteria;
using Alpha.CloneAccount.Models;
using Alpha.CloneAccount.Params;
using Alpha.CloneAccount.Utils;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Services;

public interface CustomerService
{
    Task<Result> ValidateMaxNewMember(int agentId, string language);

    Task<CustomerPTInfo> GetSMAPTInfo(UserLevel custLevel, int custId);

    Task<CopyCustomerResult> CopyCustomer(CopyCustomerParam param);

    Task<Result> CheckReachLimitDownline(CheckReachLimitDownlineParam param);

    Task<Result> CheckLimitNewDownline(CheckLimitNewDownlineParam param);
}

public class CustomerServiceImpl : CustomerService
{
    private readonly IDynamicRepository repository;
    private readonly PasswordService passwordService;
    private readonly SportbooksCopyMemberSpParamsBuilder copyMemberSpParamsBuilder;
    private readonly ThirdPartyProductService thirdPartyProductService;
    private readonly AppSettings appSettings;

    public CustomerServiceImpl(
        IDynamicRepository repository,
        PasswordService passwordService,
        SportbooksCopyMemberSpParamsBuilder copyMemberSpParamsBuilder,
        ThirdPartyProductService thirdPartyProductService,
        AppSettings appSettings)
    {
        this.repository = repository;
        this.passwordService = passwordService;
        this.copyMemberSpParamsBuilder = copyMemberSpParamsBuilder;
        this.thirdPartyProductService = thirdPartyProductService;
        this.appSettings = appSettings;
    }

    public async Task<Result> ValidateMaxNewMember(int agentId, string language)
    {
        var command = new ValidateMaxNewMemberCriteria(agentId, LanguageUtils.ToLanguageKey(language));
        await repository.ExecuteAsync(command);

        return new Result(command.ErrCode, command.ErrMsg);
    }

    public async Task<CustomerPTInfo> GetSMAPTInfo(UserLevel custLevel, int custId)
    {
        var criteria = new GetSMAPTInfoCriteria(custLevel, custId);
        var customerPTInfo = await repository.GetAsync<CustomerPTInfo>(criteria);

        if (customerPTInfo != null)
        {
            customerPTInfo.CustId = custId;
            customerPTInfo.CustLevel = custLevel.Value;
        }

        return customerPTInfo;
    }

    public async Task<CopyCustomerResult> CopyCustomer(CopyCustomerParam param)
    {
        var retryLoop = 0;
        CopyCustomerResult result;

        CheckAndGenerateRawPassword(param);

        do
        {
            result = param.ActorUserLevel.IsAgentLevel()
                ? await CopyMember(param)
                : await CopyMasterOrAgent(param);

            if (result?.Code == 201 && IsFromChatbot(param.SystemId))
            {
                retryLoop++;
                param.SetUserName(await GetAvailableDownlineUserName(param));
            }
            else
            {
                if (result?.IsSuccess == true)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(async () =>
                    {
                        await Sync3rdProductSettings(param, result.CustId);
                    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }

                return result;
            }
        } while (retryLoop < AppSettings.RetryTimesIfUserNameNotAvailable);

        return result;
    }

    private void CheckAndGenerateRawPassword(CopyCustomerParam param)
    {
        if (IsFromChatbotOrQrCode(param.SystemId, param.IsQrCode)
            && string.IsNullOrWhiteSpace(param.Password))
        {
            param.SetPassword(passwordService.Generate());
        }
    }

    private static bool IsFromChatbot(byte systemId)
        => systemId == SystemIdentity.Chatbot;

    private static bool IsFromChatbotOrQrCode(byte systemId, bool isQrCode)
        => IsFromChatbot(systemId) || isQrCode;

    private async Task<CopyCustomerResult> CopyMasterOrAgent(CopyCustomerParam param)
    {
        var criteria = BuildCopyMasterOrAgentCriteria(param);

        await repository.ExecuteAsync(criteria);

        return new CopyCustomerResult(
            criteria.CustId,
            param.UserName,
            param.Password,
            param.ActorUserLevel.GetDirectDownLineLevel().Value,
            param.SourceUsername,
            param.Credit,
            param.MaxCredit,
            criteria.ErrCode,
            criteria.ErrMsg);
    }

    private CopyCustomerCommand BuildCopyMasterOrAgentCriteria(CopyCustomerParam param)
    {
        if (param.ActorUserLevel.IsMasterLevel())
        {
            return new CopyAgentCommand(
                param.UserName,
                passwordService.CfsEnCode(param.Password),
                param.FirstName,
                param.LastName,
                param.Phone,
                param.MobilePhone,
                param.Fax,
                param.Credit,
                param.MaxCredit,
                param.ActorId,
                param.ActorId,
                param.SourceCustId,
                param.ActorSubAccId,
                param.LanguageKey,
                param.IP,
                param.SystemId,
                appSettings.SettingStatus);
        }

        return new CopyMasterCommand(
            param.UserName,
            passwordService.CfsEnCode(param.Password),
            param.FirstName,
            param.LastName,
            param.Phone,
            param.MobilePhone,
            param.Fax,
            param.Credit,
            param.ActorId,
            param.ActorId,
            param.SourceCustId,
            param.ActorSubAccId,
            param.LanguageKey,
            param.IP,
            param.SystemId,
            appSettings.SettingStatus);
    }

    private async Task<CopyCustomerResult> CopyMember(CopyCustomerParam param)
    {
        var dataTable = await GetMemberSportbooksSetting(param.SourceCustId);

        if (dataTable.Rows.Count > 0)
        {
            using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustomerInsMember_Ag");
            var spParams = copyMemberSpParamsBuilder.BuildSpParams(
                dataTable.Rows[0],
                param,
                passwordService.CfsEnCode(param.Password));

            await db.ExecuteNonQueryAsync(spParams);
            var ouputParams = db.GetOutputParams();

            return new CopyCustomerResult(
                Convert.ToInt32(ouputParams["custid"]),
                param.UserName,
                param.Password,
                param.ActorUserLevel.GetDirectDownLineLevel().Value,
                param.SourceUsername,
                param.Credit,
                param.MaxCredit,
                Convert.ToInt32(ouputParams["errcode"]),
                ouputParams["errmsg"].ToString());
        }

        return null;
    }

    private static async Task<DataTable> GetMemberSportbooksSetting(int custId)
    {
        var criteria = new GetMemberSportbooksSettingCriteria(custId);
        using IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var dataSet = await db.ExecuteDataSetAsync(criteria);

        return dataSet?.Tables?.Count == 1
            ? dataSet.Tables[0]
            : new DataTable();
    }

    public async Task Sync3rdProductSettings(CopyCustomerParam param, int createdCustId)
    {
        var createdCustomer = await GetCustomerInfo(createdCustId);
        var sourceCustomer = await GetCustomerInfo(param.SourceCustId);
        var actor = await GetCustomerInfo(param.ActorId);

        if (createdCustomer != null && sourceCustomer != null && actor != null)
        {
            if (param.ActorUserLevel.IsAgentLevel())
            {
                await thirdPartyProductService.SyncMemberSettings(
                    createdCustomer,
                    sourceCustomer,
                    actor,
                    param);
            }
            else
            {
                await thirdPartyProductService.SyncMasterOrAgentSettings(
                    createdCustomer,
                    sourceCustomer,
                    actor,
                    param);
            }
        }
    }

    private async Task<CustomerInfo> GetCustomerInfo(int custId)
        => await repository.GetAsync<CustomerInfo>(new GetCustomerInfoCriteria(custId, appSettings.SettingStatus));

    private async Task<string> GetAvailableDownlineUserName(CopyCustomerParam param)
    {
        var ptInfo = await GetSMAPTInfo(param.ActorUserLevel, param.ActorId);

        if (ptInfo != null)
        {
            var userName = param.UserName;
            var length = param.ActorUserLevel.IsAgentLevel()
                ? userName.Length - 3
                : userName.Length - 2;

            return $"{userName.Substring(0, length)}{ptInfo.Suffix}";
        }

        return string.Empty;
    }

    public async Task<Result> CheckReachLimitDownline(CheckReachLimitDownlineParam param)
    {
        var command = new CheckReachLimitDownlineCommand(
            param.CustId,
            param.CustLevel,
            param.Username,
            param.LanguageKey);

        await repository.ExecuteAsync(command);

        return new Result(command.ErrCode, command.ErrMsg);
    }

    public async Task<Result> CheckLimitNewDownline(CheckLimitNewDownlineParam param)
    {
        var command = new CheckLimitNewDownlineCommand(param.CustId);
        await repository.ExecuteAsync(command);

        return command.IsAllowed
            ? new Result(0, string.Empty)
            : GetLimitNewDownlineErrorResult(param.CustLevel, param.Language);
    }

    private static Result GetLimitNewDownlineErrorResult(UserLevel custLevel, string language)
    {
        const int LimitNewDownlineErrorCode = 1001;

        return new Result(
            LimitNewDownlineErrorCode,
            StaticResources.Get(GetLimitNewDownlineResourceKey(custLevel), language));
    }

    private static string GetLimitNewDownlineResourceKey(UserLevel custLevel)
    {
        if (custLevel.IsSuperLevel())
        {
            return "LimitSuperNewDownlineError";
        }

        if (custLevel.IsMasterLevel())
        {
            return "LimitMasterNewDownlineError";
        }

        return "LimitAgentNewDownlineError";
    }
}
