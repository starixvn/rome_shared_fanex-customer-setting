using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomrSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;


public interface ICreateMasterFullModeService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}

public class CreateMasterFullModeService : ICreateMasterFullModeService
{

    private const string CheckPermissionSPName = "Acc_GetDeniedSiteCurrencyByUser";

    private const string ValidateSPName = "Acc_CustomerInsMember_SAg_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CreateNewSPName = "Acc_CustomerInsMember_SAg";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    private readonly IScaleoutDBService scaleoutDBService;

    private readonly IMessageService messageService;

    private readonly AppSettings appSettings;

    private readonly ICustomerSettingHelper helperService;

    public CreateMasterFullModeService(IMessageService messageService, 
        AppSettings appSettings, 
        IScaleoutDBService scaleoutDBService,
        ICustomerSettingHelper helperService)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.scaleoutDBService = scaleoutDBService;
        this.helperService = helperService;
    }

    public async Task<CreateCustomerResult> Create(CreateRequest request)
    {
        var result = await CheckPermission(request);
        if (!result.IsSuccessful)
        {
            return result;
        }
        
        result = await ValidateSetting(request);
        if (!result.IsSuccessful)
        {
            return result;
        }
        
        result = await Register(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.NewCustId;
        result = await CreateMaster(request, newCustId);
        if (!result.IsSuccessful)
        {
            Rollback(newCustId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> CheckPermission(CreateRequest request)
    {
        var parameters = new Hashtable
        {
            { "site", request.Settings.GeneralInfo.Site },
            { "currency", request.Settings.GeneralInfo.Currency },
            { "creator", request.Context.CreatorId },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(CheckPermissionSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var parameters = new Hashtable
        {
            { "site", request.Settings.GeneralInfo.Site },
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "srecommend", request.Settings.GeneralInfo.UplineCustId },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "ip", request.Context.IP },
            { "settingstatus", appSettings.SettingStatus },
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
            { "systemid", request.Context.SystemId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(ValidateSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> Register(CreateRequest request)
    {
        var parameters = new Hashtable
        {
            { "username", request.Settings.GeneralInfo.UserName },
            { "sitedbgroupid", await scaleoutDBService.GetCustDBGroupIdBySite(request.Context.Site) },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(Convert.ToInt32(outputParams["custid"]), Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> CreateMaster(CreateRequest request, int newCustId)
    {
        var parameters = new Hashtable
        {
            { "custid", newCustId },
            { "firstname", request.Settings.GeneralInfo.FirstName ?? string.Empty },
            { "lastname", request.Settings.GeneralInfo.LastName ?? string.Empty},
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "userpwd", request.Settings.GeneralInfo.Password },
            { "phone", request.Settings.GeneralInfo.Phone ?? string.Empty},
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone ?? string.Empty},
            { "fax", request.Settings.GeneralInfo.Fax ?? string.Empty},
            { "transferoption", request.Settings.GeneralInfo.TransferOption},
            { "disableautompt", request.Settings.GeneralInfo.DisableAutoMPT},
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "srecommend", request.Settings.GeneralInfo.UplineCustId },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "site", request.Settings.GeneralInfo.Site },
            { "settingstatus", appSettings.SettingStatus },
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(CreateNewSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(newCustId, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

    private async void Rollback(int newCustId)
    {
        var parameters = new Hashtable
        {
            { "custid", newCustId }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RollbackSPName);
        await db.ExecuteNonQueryAsync(parameters);
    }

}
