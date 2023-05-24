using System.Data;
using Fanex.Customer.API.Modules.CopyCustomer.Command;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface ICopyCustomerService
{
    Task<CopyMultipleCustomerResult> CopyCustomer(CopyCustomerRequest request);

    Task<Result> CheckLimitNewDownline(CheckLimitNewDownlineRequest request);
}

public class CopyCustomerService : ICopyCustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly AppSettings appSettings;
    private readonly ICopyMemberSpParamsBuilder copyMemberSpParamsBuilder;
    private readonly IThirdPartyProductService thirdPartyProductService;
    private readonly ICustomerService customerService;
    private readonly Logging.ILogger logger;

    private readonly IBalanceCustomerService balanceCustomerService;

    public CopyCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ICopyMemberSpParamsBuilder copyMemberSpParamsBuilder,
        IThirdPartyProductService thirdPartyProductService,
        ICustomerService customerService,
        Logging.ILogger logger,
        IBalanceCustomerService balanceCustomerService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.copyMemberSpParamsBuilder = copyMemberSpParamsBuilder;
        this.thirdPartyProductService = thirdPartyProductService;
        this.customerService = customerService;
        this.logger = logger;
        this.balanceCustomerService = balanceCustomerService;
    }

    public async Task<CopyMultipleCustomerResult> CopyCustomer(CopyCustomerRequest request)
    {
        var checkLimitNewDownlineResult = await CheckLimitNewDownline(new CheckLimitNewDownlineRequest
        {
            CustId = request.ActorId,
            CustLevel = request.ActorUserLevel,
            Language = request.Language
        });
        if (!checkLimitNewDownlineResult.IsSuccessful)
        {
            return new CopyMultipleCustomerResult(Enumerable.Empty<string>(), checkLimitNewDownlineResult.Code, checkLimitNewDownlineResult.Message);
        }

        var isSourceAccountEnable = request.SourceCustId > 0 && await customerService.IsEnableCustomer(request.SourceCustId);

        if (!isSourceAccountEnable)
        {
            return new CopyMultipleCustomerResult(Enumerable.Empty<string>(), 100, StaticResources.Get("ThisSourceAccountWasDisabled", request.Language));
        }

        var index = 0;
        var retryLoop = 0;
        var usernames = new List<string>();

        do
        {
            var result = request.ActorLevel.IsAgentLevel() ? await CopyMember(request) : await CopyMasterOrAgent(request);
            if (result.IsSuccessful)
            {
                index++;
                retryLoop = 0;
                usernames.Add(request.UserName);
                request.UserName = await GetAvailableDownlineUserName(request);

                Task.Run(async () =>
                {
                    await Sync3rdProductSettings(request, result.CustId);
                });
            }
            else
            {
                if (result.Code == 201 && retryLoop < 5)
                {
                    retryLoop++;
                    request.UserName = await GetAvailableDownlineUserName(request);
                }
                else
                {
                    return new CopyMultipleCustomerResult(usernames, result.Code, result.Message);
                }
            }

        } while (index < request.NumberOfAccounts);

        return new CopyMultipleCustomerResult(usernames);
    }

    private async Task Sync3rdProductSettings(CopyCustomerRequest param, int createdCustId)
    {
        var createdCustomer = await customerService.GetCustomerInfo(createdCustId);
        var sourceCustomer = await customerService.GetCustomerInfo(param.SourceCustId);
        var actor = await customerService.GetCustomerInfo(param.ActorId);

        if (createdCustomer != null && sourceCustomer != null && actor != null && param.ActorLevel.IsAgentLevel())
        {
            await thirdPartyProductService.SyncMemberSettings(
                createdCustomer,
                sourceCustomer,
                actor,
                param,
                appSettings.SettingStatus);
        }
    }

    private async Task<CopyCustomerResult> CopyMember(CopyCustomerRequest param)
    {
        var dataTable = await GetMemberSetting(param.SourceCustId);

        if (dataTable.Rows.Count > 0)
        {
            using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustomerInsMember_Ag");
            var spParams = copyMemberSpParamsBuilder.BuildSpParams(
                dataTable.Rows[0],
                param,
                param.Password);

            await db.ExecuteNonQueryAsync(spParams);
            var ouputParams = db.GetOutputParams();
            var errCode = Convert.ToInt32(ouputParams["errcode"]);
            var custId = Convert.ToInt32(ouputParams["custid"]);

            if (errCode == 0)
            {
                await balanceCustomerService.InitBalanceCustomer(custId);
            }

            return new CopyCustomerResult(custId, errCode, ouputParams["errmsg"].ToString());
        }

        return null;
    }

    private static async Task<DataTable> GetMemberSetting(int custId)
    {
        var criteria = new GetMemberSettingCriteria(custId);
        using IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var dataSet = await db.ExecuteDataSetAsync(criteria);

        return dataSet?.Tables?.Count == 1
            ? dataSet.Tables[0]
            : new DataTable();
    }

    private async Task<CopyCustomerResult> CopyMasterOrAgent(CopyCustomerRequest request)
    {
        var criteria = BuildCopyMasterOrAgentCriteria(request);

        await dynamicRepository.ExecuteAsync(criteria);

        if (criteria.CustId > 0)
        {
            InsertUnreadMessage(criteria.CustId, request.ActorLevel.GetDirectDownLineLevel().Value);
        }

        return new CopyCustomerResult(
            criteria.CustId,
            criteria.ErrCode,
            criteria.ErrMsg);
    }

    private CopyCustomerCommand BuildCopyMasterOrAgentCriteria(CopyCustomerRequest request)
    {
        if (request.ActorLevel.IsMasterLevel())
        {
            return new CopyAgentCommand(
                request.UserName,
                request.Password,
                request.Credit,
                request.MaxCredit,
                request.ActorId,
                request.ActorId,
                request.SourceCustId,
                request.ActorSubAccId,
                request.LanguageKey,
                request.IP,
                request.SystemId,
                appSettings.SettingStatus);
        }

        return new CopyMasterCommand(
            request.UserName,
            request.Password,
            request.Credit,
            request.ActorId,
            request.ActorId,
            request.SourceCustId,
            request.ActorSubAccId,
            request.LanguageKey,
            request.IP,
            request.SystemId,
            appSettings.SettingStatus);
    }

    private async Task<string> GetAvailableDownlineUserName(CopyCustomerRequest request)
    {
        var length = request.ActorLevel.IsAgentLevel()
                ? request.UserName.Length - 3
                : request.UserName.Length - 2;
        var userName = request.UserName.Substring(0, length);
        var suffix = await customerService.GetSuggestUsername(new GetSuggestUsernameRequest { ActorName = userName, RoleId = request.ActorUserLevel });

        return !string.IsNullOrWhiteSpace(suffix) ? $"{userName}{suffix}" : string.Empty;
    }

    public async Task<Result> CheckLimitNewDownline(CheckLimitNewDownlineRequest request)
    {
        var command = new CheckLimitNewDownlineCommand(request.CustId);
        await dynamicRepository.ExecuteAsync(command);

        return command.IsAllowed
            ? Result.CreateSuccessfulResult()
            : GetLimitNewDownlineErrorResult(Enumeration.FromValue<UserLevel>(request.CustLevel), request.Language);
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

    private void InsertUnreadMessage(int custId, byte roleId)
    {
        Task.Run(() =>
        {
            try
            {
                var criteria = new CustomerReadMessageCommand(custId, roleId, 0, 0, string.Empty);
                this.dynamicRepository.Execute(criteria);

                if (criteria.ErrCode != 0)
                {
                    logger.Info($"[Age_MsgCustReadStatus_Ins] CustId: {custId}; SubAccId: 0; RoleId: {roleId} ErrCode: {criteria.ErrCode}; ErrMsg: {criteria.ErrMsg}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        });
    }
}