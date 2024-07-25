using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomrSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;


public interface ICreateAgentFullModeService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}

public class CreateAgentFullModeService : ICreateAgentFullModeService
{

    private const string ValidateSPName = "Acc_CustomerInsMember_MAg_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CreateNewSPName = "Acc_CustomerInsMember_MAg";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    private readonly IScaleoutDBService scaleoutDBService;

    private readonly IMessageService messageService;

    private readonly AppSettings appSettings;

    private readonly ICustomerSettingHelper helperService;

    public CreateAgentFullModeService(IMessageService messageService, 
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
        var result = await ValidateSetting(request);
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
        result = await CreateAgent(request, newCustId);
        if (!result.IsSuccessful)
        {
            Rollback(newCustId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var parameters = new Hashtable
        {
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "mrecommend", request.Settings.GeneralInfo.UplineCustId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "maxcredit", request.Settings.GeneralInfo.MemberMaxCredit },
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "ip", request.Context.IP },
            { "settingstatus", appSettings.SettingStatus },
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
            { "systemid", request.Context.SystemId },
            { "site", request.Settings.GeneralInfo.Site },
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

    private async Task<CreateCustomerResult> CreateAgent(CreateRequest request, int newCustId)
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
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "maxcredit", request.Settings.GeneralInfo.MemberMaxCredit },
            { "mrecommend", request.Settings.GeneralInfo.UplineCustId },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
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
