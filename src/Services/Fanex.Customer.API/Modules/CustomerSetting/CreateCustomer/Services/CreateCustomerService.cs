using Fanex.Customer.API.Modules.Balance;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Extensions;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Services.LiteMode;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.Password.Service;
using Fanex.Common.Extensions;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    bool ValidateRequest(CreateRequest request);

    Task<CustomerSettingResult> Create(CreateRequest request);
}

public class CreateCustomerService : CreateCustomerServiceBase, ICreateCustomerService
{
    private readonly IBalanceService balanceService;
    private readonly ICustomerService customerService;

    public CreateCustomerService(
        IBalanceService balanceService,
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISettingProfileService sharedService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        ICustomerSettingHelper helperService,
        ICustomerService customerService,
        IEncryption encryption,
        HttpClient httpClient)
        : base(dynamicRepository, appSettings, sharedService, cacheRefService, messageService, null, helperService, encryption, httpClient)
    {
        this.balanceService = balanceService;
        this.customerService = customerService;
    }

    public bool ValidateRequest(CreateRequest request)
    {
        if(request.Settings.GeneralInfo == null || request.Settings.GeneralInfo.RoleId == 0 || request.Settings.GeneralInfo.UplineRoleId == 0)
        {
            return false;
        }

        if ((!string.IsNullOrWhiteSpace(request.Settings.GeneralInfo.FirstName) && request.Settings.GeneralInfo.FirstName.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(request.Settings.GeneralInfo.LastName) && request.Settings.GeneralInfo.LastName.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(request.Settings.GeneralInfo.Phone) && request.Settings.GeneralInfo.Phone.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(request.Settings.GeneralInfo.MobilePhone) && request.Settings.GeneralInfo.MobilePhone.ContainsHTML())
        || (!string.IsNullOrWhiteSpace(request.Settings.GeneralInfo.Fax) && request.Settings.GeneralInfo.Fax.ContainsHTML()))
        {
            return false;
        }

        if (!appSettings.IsSupportPT3Decimal(request.Context.SiteLabel))
        {
            foreach (var productGroup in request.Settings.ProductGroups)
            {
                if (productGroup.GroupTypes.Any(gt => gt.Id == 153 && gt.GroupItems.Any(gi => gi.Is3DecimalPTValue())))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public async Task<CustomerSettingResult> Create(CreateRequest request)
    {
        CreateCustomerResult result;
        var roleId = request.Settings.GeneralInfo.RoleId;
        var uplineRoleId = request.Settings.GeneralInfo.UplineRoleId;
        request.Settings.GeneralInfo.Password = await HashPassword(request.Settings.GeneralInfo.Password);
        var isDirectMember = helperService.IsCreateNewDirectMember(roleId, uplineRoleId, null);
        var createCustomerService = GetCreateCustomerService(request.Context.IsFullMode(), request.Settings.GeneralInfo.UplineRoleId, isDirectMember);
        result = await createCustomerService.Create(request);

        if (result.IsSuccessful && (Enumeration.FromValue<UserLevel>(uplineRoleId).IsAgentLevel() || isDirectMember))
        {
            await balanceService.InitBalance(request.Settings.GeneralInfo.Site, result.NewCustId);
        }

        if (result.IsSuccessful)
        {
            AfterCreateCustomerProcess(
                result.NewCustId,
                request.Context.SubAccId,
                (byte)(uplineRoleId - 1),
                request.Settings.GeneralInfo.SiteLabel,
                request.Settings.GeneralInfo.Currency,
                request.Settings.GeneralInfo.UserName,
                request.Context.CreatorId,
                request.Context.IP);
        }

        if (result.IsSuccessful)
        {
            result.AvailableAlphaDomain = await customerService.GetBackupDomains(
                request.Settings.GeneralInfo.Site,
                isDirectMember ? CustomerLevel.Member : (byte)(request.Settings.GeneralInfo.UplineRoleId - 1));
        }

        return result;
    }

    private CreateCustomerBaseService GetCreateCustomerService(bool isFullMode, int uplineRoleId, bool? isDirectMember)
    {
        if(isDirectMember.GetValueOrDefault())
        {
            return uplineRoleId switch
            {
                CustomerLevel.Super => new CreateDirectMemberService(messageService, appSettings, helperService, customerService),
                CustomerLevel.Master => new CreateDirectMemberService(messageService, appSettings, helperService, customerService),
                _=> null
            };
        }

        if (isFullMode)
        {
            return uplineRoleId switch
            {
                CustomerLevel.Company => new CreateSuperFullModeService(messageService, appSettings, helperService),
                CustomerLevel.Super => new CreateMasterFullModeService(messageService, appSettings, helperService),
                CustomerLevel.Master => new CreateAgentFullModeService(messageService, appSettings, helperService),
                CustomerLevel.Agent => new CreateMemberFullModeService(messageService, appSettings, helperService),
                _ => null
            };
        }
        else
        {
            return uplineRoleId switch
            {
                CustomerLevel.Super => new CreateMasterLiteModeService(messageService, appSettings, helperService),
                CustomerLevel.Master => new CreateAgentLiteModeService(messageService, appSettings, helperService),
                CustomerLevel.Agent => new CreateMemberLiteModeService(messageService, appSettings, helperService),
                _ => null
            };
        }
    }

    private void AfterCreateCustomerProcess(int newCustId, int subAccId, byte roleId, string siteLabel, int currencyId, string userName, int userId, string ip)
    {
        AddMsgAfterCreateNewSMA(new AddMsgAfterCreateNewSMACommand(newCustId, subAccId, roleId));
        if (roleId == CustomerLevel.Super && appSettings.IsSpecialSites(siteLabel) && currencyId != 20)
        {
            AddSuperToAutoTransferList(new InsertSuperToAutoTransferCommand(userName, 2, userId, ip));
            AddDefaultDateLimitSuper(new UpdateCustomerDateLimitCommand(newCustId.ToString(), userName, null, 33, 2, userId));
        }
    }

    private async void AddDefaultDateLimitSuper(UpdateCustomerDateLimitCommand command)
    {
        try
        {
            await dynamicRepository.ExecuteAsync(command);
        }
        catch (Exception ex)
        {
            Fanex.Logging.LogService.Error(ex.Message, ex);
        }
    }

    private async void AddSuperToAutoTransferList(InsertSuperToAutoTransferCommand command)
    {
        try
        {
            await dynamicRepository.ExecuteAsync(command);
        }
        catch (Exception ex)
        {
            Fanex.Logging.LogService.Error(ex.Message, ex);
        }
    }

    private async void AddMsgAfterCreateNewSMA(AddMsgAfterCreateNewSMACommand command)
    {
        try
        {
            await dynamicRepository.ExecuteAsync(command);
            if (command.ErrCode > 0 || !string.IsNullOrWhiteSpace(command.ErrMsg))
            {
                Fanex.Logging.LogService.Info(string.Format("[Age_MsgCustReadStatus_Ins] CustId: {0}; SubAccId: {1}; RoleId: {2} ErrCode: {3}; ErrMsg: {4}",
                    command.CustId,
                    command.SubAccId,
                    command.RoleId,
                    command.ErrCode,
                    command.ErrMsg));
            }
        }
        catch (Exception ex)
        {
            Fanex.Logging.LogService.Error(ex.Message, ex);
        }
    }
}