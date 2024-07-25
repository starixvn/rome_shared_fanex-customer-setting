using System.Collections;
using Fanex.Customer.API.Modules.Balance;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;
using Fanex.Customer.API.Modules.CreateCustomer.SharedServices;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomrSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Extensions;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerService
{
    bool ValidateRequest(CreateRequest request);

    Task<CustomerSettingResult> Create(CreateRequest request);
}

public class CreateCustomerService : BaseCustomerService, ICreateCustomerService
{
    private readonly IBalanceService balanceService;
    private readonly ICustomerService customerService;
    private readonly IScaleoutDBService scaleoutDBService;

    public CreateCustomerService(
        IBalanceService balanceService,
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ISharedService sharedService,
        ICacheRefService cacheRefService,
        IMessageService messageService,
        ICustomerSettingHelper helperService,
        ICustomerService customerService,
        IScaleoutDBService scaleoutDBService)
        )
        : base(dynamicRepository, appSettings, sharedService, cacheRefService, messageService, null, helperService)
    {
        this.balanceService = balanceService;
        this.customerService = customerService;
        this.scaleoutDBService = scaleoutDBService;
    }

    public bool ValidateRequest(CreateRequest request)
    {
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
        var uplineRoleId = request.Settings.GeneralInfo.UplineRoleId;
        if (Enumeration.FromValue<UserLevel>(uplineRoleId).IsCompanyLevel())
        {
            CreateSuperFullModeService createSuperFullModeService = new CreateSuperFullModeService(messageService, appSettings, scaleoutDBService, helperService);
            result = await createSuperFullModeService.Create(request);
        }
        else
        {
            var parameters = GetCreateCustomerParams(request, appSettings);
            using IObjectDb db = ObjectDbFactory.CreateInstance(GetCreateNewSPName(uplineRoleId, request.Context.IsFullMode()));
            await db.ExecuteNonQueryAsync(parameters);
            var outputParams = db.GetOutputParams();
            var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
            result = new CreateCustomerResult(Convert.ToInt32(outputParams["custid"]), Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
        }

        if (result.IsSuccessful && Enumeration.FromValue<UserLevel>(uplineRoleId).IsAgentLevel())
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
                (byte)(request.Settings.GeneralInfo.UplineRoleId  - 1));
        }

        return result;
    }

    private Hashtable GetCreateCustomerParams(CreateRequest request, AppSettings appSettings)
    {
        var param = CreateGeneralInfoParams(request, appSettings);

        param.Add("SettingXML", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()));
        return param;
    }

    private Hashtable CreateGeneralInfoParams(CreateRequest request, AppSettings appSettings)
    {
        var uplineRoleId = request.Settings.GeneralInfo.UplineRoleId;

        var parameters = new Hashtable {
            { "firstname", request.Settings.GeneralInfo.FirstName ?? string.Empty },
            { "lastname", request.Settings.GeneralInfo.LastName ?? string.Empty},
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "userpwd", request.Settings.GeneralInfo.Password },
            { "phone", request.Settings.GeneralInfo.Phone ?? string.Empty},
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone ?? string.Empty},
            { "fax", request.Settings.GeneralInfo.Fax ?? string.Empty},
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "settingstatus", appSettings.SettingStatus },
        };

        if (!request.Context.IsFullMode())
        {
            parameters.Add("settingtypeall", (request.Context.UIBehavior != null) ? helperService.ObjectToXml(request.Context.UIBehavior) : string.Empty);
            parameters.Add("templatename", request.Settings.GeneralInfo.TemplateName);
        }

        if (request.Context.IsFullMode() && uplineRoleId == CustomerLevel.Company)
        {
            parameters.Add("isinternal", request.Settings.GeneralInfo.IsInternal);
        }

        if (uplineRoleId == CustomerLevel.Company)
        {
            parameters.Add("site", request.Settings.GeneralInfo.Site);
            parameters.Add("currency", request.Settings.GeneralInfo.Currency);
        }
        else
        {
            parameters.Add("subaccid", request.Context.SubAccId);
        }

        if (uplineRoleId == CustomerLevel.Super)
        {
            parameters.Add("disableautompt", request.Settings.GeneralInfo.DisableAutoMPT);
            parameters.Add("srecommend", request.Settings.GeneralInfo.UplineCustId);
            parameters.Add("transferoption", request.Settings.GeneralInfo.TransferOption);
            parameters.Add("site", request.Settings.GeneralInfo.Site);
        }

        if (uplineRoleId == CustomerLevel.Master)
        {
            parameters.Add("maxcredit", request.Settings.GeneralInfo.MemberMaxCredit);
            parameters.Add("mrecommend", request.Settings.GeneralInfo.UplineCustId);
            parameters.Add("transferoption", request.Settings.GeneralInfo.TransferOption);
        }

        if (uplineRoleId == CustomerLevel.Agent)
        {
            parameters.Add("recommend", request.Settings.GeneralInfo.UplineCustId);
            parameters.Add("ugroup", request.Settings.GeneralInfo.Group);
        }

        return parameters;
    }

    private static string GetCreateNewSPName(int roleId, bool isFullMode)
        => roleId switch
        {
            CustomerLevel.Super => isFullMode ? "Acc_CustomerInsMember_Sag" : "Acc_LiteMode_CustomerInsMember_SAg",
            CustomerLevel.Master => isFullMode ? "Acc_CustomerInsMember_MAg" : "Acc_LiteMode_CustomerInsMember_MAg",
            CustomerLevel.Agent => isFullMode ? "Acc_CustomerInsMember_Ag" : "Acc_LiteMode_CustomerInsMember_Ag",
            _ => isFullMode ? "Acc_CustomerInsMember_Com" : throw new IndexOutOfRangeException($"RoleId cannot = {roleId}")
        };

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