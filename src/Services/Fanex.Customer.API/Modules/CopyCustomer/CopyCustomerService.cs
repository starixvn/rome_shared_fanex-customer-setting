using Fanex.Customer.API.Modules.Balance;
using Fanex.Customer.API.Modules.CopyCustomer.Command;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.CopyCustomer.Services;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Customer.Requests;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Resources;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
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
    private readonly IScaleoutDBService scaleoutDBService;

    public CopyCustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ICustomerService customerService,
        Logging.ILogger logger,
        IBalanceService balanceService,
        IMessageService messageService,
        IScaleoutDBService scaleoutDBService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.customerService = customerService;
        this.logger = logger;
        this.balanceService = balanceService;
        this.messageService = messageService;
        this.scaleoutDBService = scaleoutDBService;
    }

    public async Task<CopyMultipleCustomerResult> CopyCustomers(CopyMultipleCustomersRequest request)
    {
        var isSourceAccountEnable = request.SourceCustId > 0 && await customerService.IsEnableCustomer(request.SourceCustId);
        var domains = await customerService.GetBackupDomains(request.Site, request.CustLevel);

        if (!isSourceAccountEnable)
        {
            return new CopyMultipleCustomerResult(
                Enumerable.Empty<string>(),
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
                    return new CopyMultipleCustomerResult(usernames, domains, suggestUsernameResult.Code, mainMessage: suggestUsernameResult.Message);
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
                        return new CopyMultipleCustomerResult(usernames, domains, suggestUsernameResult.Code, messageContent: content);
                    }

                    request.UserName = $"{request.ActorUsername}{suggestUsernameResult.CustomData}";
                }
                else
                {
                    return new CopyMultipleCustomerResult(usernames, domains, result.Code, result.MsgType, messageContent: result.MessageContent);
                }
            }

        } while (index < request.NumberOfAccounts);

        return new CopyMultipleCustomerResult(usernames, domains, 0, 0);
    }

    public async Task<CopyCustomerResult> CopyCustomer(CopyCustomerRequest request)
    {
        var copyService = this.CreateCopyCustomerService(request.CustLevel);
        var result = await copyService.Copy(request);

        if (result.CustId > 0)
        {
            if (request.CustLevel == CustomerLevel.Member)
            {
                await balanceService.InitBalance(request.Site, result.CustId);
            }

            _ = Task.Run(() => InsertUnreadMessage(result.CustId, request.CustLevel));
        }

        return new CopyCustomerResult(
                result.CustId,
                result.Code,
                result.MsgType,
                result.MessageContent);
    }

    private CopyCustomerBaseService CreateCopyCustomerService(int roleId)
    {
        return roleId switch
        {
            CustomerLevel.Super => new CopySuperService(messageService, appSettings, scaleoutDBService),
            CustomerLevel.Master => new CopyMasterService(messageService, appSettings, scaleoutDBService),
            CustomerLevel.Agent => new CopyAgentService(messageService, appSettings, scaleoutDBService),
            CustomerLevel.Member => new CopyMemberService(messageService, appSettings, scaleoutDBService),
            _ => throw new ArgumentOutOfRangeException(nameof(roleId))
        };
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