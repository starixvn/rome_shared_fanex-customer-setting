using Fanex.Customer.API.Modules.Balance;
using Fanex.Customer.API.Modules.CopyCustomer.Command;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CopyCustomer;

public interface ICopyCustomerService
{
    Task<CopyMultipleCustomerResult> CopyCustomers(CopyMultipleCustomersRequest request);

    Task<CopyCustomerResult> CopyCustomer(CopyCustomerRequest request);
}

public class CopyCustomerService : ICopyCustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly AppSettings appSettings;
    private readonly ICustomerService customerService;
    private readonly Logging.ILogger logger;
    private readonly IBalanceService balanceService;
    private readonly IMessageService messageService;

    public CopyCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ICustomerService customerService,
        Logging.ILogger logger,
        IBalanceService balanceService,
        IMessageService messageService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.customerService = customerService;
        this.logger = logger;
        this.balanceService = balanceService;
        this.messageService = messageService;
    }

    public async Task<CopyMultipleCustomerResult> CopyCustomers(CopyMultipleCustomersRequest request)
    {
        var isSourceAccountEnable = request.SourceCustId > 0 && await customerService.IsEnableCustomer(request.SourceCustId);

        if (!isSourceAccountEnable)
        {
            return new CopyMultipleCustomerResult(
                Enumerable.Empty<string>(),
                100,
                messageContent: new MessageContent(StaticResources.Get("ThisSourceAccountWasDisabled", request.Language)));
        }

        var index = 0;
        var retryLoop = 0;
        var usernames = new List<string>();

        do
        {
            var suggestUsernameResult = Result.CreateSuccessfulResult();
            var result = await CopyCustomer(request);
            if (result.IsSuccessful)
            {
                index++;
                retryLoop = 0;

                if (!string.IsNullOrWhiteSpace(request.UserName))
                {
                    usernames.Add(request.UserName);
                }

                suggestUsernameResult = await GetAvailableDownlineUserName(request);

                if (!suggestUsernameResult.IsSuccessful)
                {
                    return new CopyMultipleCustomerResult(usernames, suggestUsernameResult.Code, mainMessage: suggestUsernameResult.Message);
                }

                request.UserName = $"{request.ActorUsername}{suggestUsernameResult.CustomData}";
            }
            else
            {
                if (result.Code == 201 && retryLoop < 5)
                {
                    retryLoop++;
                    suggestUsernameResult = await GetAvailableDownlineUserName(request);

                    if (!suggestUsernameResult.IsSuccessful)
                    {
                        var content = new MessageContent(suggestUsernameResult.Message);
                        return new CopyMultipleCustomerResult(usernames, suggestUsernameResult.Code, messageContent: content);
                    }

                    request.UserName = $"{request.ActorUsername}{suggestUsernameResult.CustomData}";
                }
                else
                {
                    return new CopyMultipleCustomerResult(usernames, result.Code, result.MsgType, messageContent: result.MessageContent);
                }
            }

        } while (index < request.NumberOfAccounts);

        return new CopyMultipleCustomerResult(usernames, 0, 0);
    }

    public async Task<CopyCustomerResult> CopyCustomer(CopyCustomerRequest request)
    {
        var criteria = BuildCriteria(request);

        using var objDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());

        //await objDb.ExecuteNonQueryAsync(criteria);
        await dynamicRepository.ExecuteAsync(criteria);

        if (criteria.CustId > 0)
        {
            if (request.CustLevel == CustomerLevel.Member)
            {
                await balanceService.InitBalance(request.Site, criteria.CustId);
            }

            Task.Run(() => InsertUnreadMessage(criteria.CustId, request.CustLevel));
        }

        if (criteria.GetType() == typeof(CopyMemberCommand))
        {
            var copyMemberCriteria = (CopyMemberCommand)criteria;
            return new CopyCustomerResult(
                criteria.CustId,
                criteria.ErrCode,
                copyMemberCriteria.MsgType,
                messageService.TranslateMessage(copyMemberCriteria.MsgType, criteria.ErrMsg, request.Language));

        }

        return new CopyCustomerResult(
            criteria.CustId,
            criteria.ErrCode,
            mainMessage: criteria.ErrMsg);
    }

    private CopyCustomerCommand BuildCriteria(CopyCustomerRequest request)
    {
        CopyCustomerCommand criteria = request.CustLevel switch
        {
            CustomerLevel.Super => new CopySuperCommand(
                                        request.Site,
                                        request.IsInternal,
                                        request.UserName,
                                        request.Password,
                                        request.Credit,
                                        request.ActorId,
                                        request.SourceCustId,
                                        request.LanguageKey,
                                        request.IP,
                                        request.SystemId,
                                        appSettings.SettingStatus),
            CustomerLevel.Master => new CopyMasterCommand(
                                        request.UserName,
                                        request.Password,
                                        request.Credit,
                                        request.ActorId,
                                        request.UplineId,
                                        request.SourceCustId,
                                        request.ActorSubAccId,
                                        request.LanguageKey,
                                        request.IP,
                                        request.SystemId,
                                        appSettings.SettingStatus),
            CustomerLevel.Agent => new CopyAgentCommand(
                                        request.UserName,
                                        request.Password,
                                        request.Credit,
                                        request.MaxCredit,
                                        request.ActorId,
                                        request.UplineId,
                                        request.SourceCustId,
                                        request.ActorSubAccId,
                                        request.LanguageKey,
                                        request.IP,
                                        request.SystemId,
                                        appSettings.SettingStatus),
            CustomerLevel.Member => new CopyMemberCommand(
                                        request.UserName,
                                        request.Password,
                                        request.Credit,
                                        request.IsQrCode,
                                        request.ActorId,
                                        request.UplineId,
                                        request.SourceCustId,
                                        request.ActorSubAccId,
                                        request.LanguageKey,
                                        request.IP,
                                        request.SystemId,
                                        appSettings.SettingStatus),
            _ => throw new ArgumentOutOfRangeException("CustLevel", $"Not expected CustLevel value: {request.CustLevel}")
        };

        criteria.FirstName = request.FirstName ?? string.Empty;
        criteria.LastName = request.LastName ?? string.Empty;
        criteria.Phone = request.Phone ?? string.Empty;
        criteria.MobilePhone = request.MobilePhone ?? string.Empty;
        criteria.Fax = request.Fax ?? string.Empty;

        return criteria;
    }


    private async Task<Result> GetAvailableDownlineUserName(CopyCustomerRequest request)
    {
        var roleId = Enumeration.FromValue<UserLevel>(request.CustLevel).GetDirectUpLineLevel().Value;
        var result = await customerService.GetSuggestUsername(new GetSuggestUsernameRequest { ActorName = request.ActorUsername, RoleId = roleId });

        return result;
    }

    private async Task InsertUnreadMessage(int custId, byte roleId)
    {
        try
        {
            var criteria = new CustomerReadMessageCommand(custId, roleId, 0, 0, string.Empty);
            await dynamicRepository.ExecuteAsync(criteria);

            if (criteria.ErrCode != 0)
            {
                await logger.InfoAsync($"[Age_MsgCustReadStatus_Ins] CustId: {custId}; SubAccId: 0; RoleId: {roleId} ErrCode: {criteria.ErrCode}; ErrMsg: {criteria.ErrMsg}");
            }
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
        }
    }
}