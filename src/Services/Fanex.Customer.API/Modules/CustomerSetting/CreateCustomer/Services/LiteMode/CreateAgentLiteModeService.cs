using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CreateCustomer.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Services.LiteMode;

public class CreateAgentLiteModeService : CreateCustomerBaseService, ICreateCustomerBaseService
{

    private const string ValidateSPName = "Acc_LiteMode_CustomerInsMember_MAg_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CreateNewSPName = "Acc_LiteMode_CustomerInsMember_MAg";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public CreateAgentLiteModeService(IMessageService messageService, 
        AppSettings appSettings, 
        ICustomerSettingHelper helperService,
        IDynamicRepository dynamicRepository)
        : base( messageService, appSettings, helperService, dynamicRepository)
    {
    }

    public override async Task<CreateCustomerResult> Create(CreateRequest request)
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

        int newCustId = result.NewCustIds[0];
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
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "maxcredit", request.Settings.GeneralInfo.MemberMaxCredit },
            { "mrecommend", request.Settings.GeneralInfo.UplineCustId },
            { "ip", request.Context.IP },
            { "systemid", request.Context.SystemId },
            { "settingstatus", appSettings.SettingStatus },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(ValidateSPName, request.Context.CustDBGroupID));
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
            { "sitedbgroupid", request.Context.CustDBGroupID },
            { "site", request.Settings.GeneralInfo.Site },
            { "roleid", CustomerLevel.Agent },
            { "uplineid", request.Settings.GeneralInfo.UplineCustId },
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
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
            { "templatename", request.Settings.GeneralInfo.TemplateName},
            { "settingtypeall",  helperService.ObjectToXml(request.Context.UIBehavior)},
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "maxcredit", request.Settings.GeneralInfo.MemberMaxCredit },
            { "mrecommend", request.Settings.GeneralInfo.UplineCustId },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "settingstatus", appSettings.SettingStatus },
            { "remark", request.Settings.GeneralInfo.Remark }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(CreateNewSPName, request.Context.CustDBGroupID));
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