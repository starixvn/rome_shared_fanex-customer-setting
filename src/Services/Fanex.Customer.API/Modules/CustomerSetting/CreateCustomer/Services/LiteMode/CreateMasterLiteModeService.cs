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

public class CreateMasterLiteModeService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    private const string ValidateSPName = "Acc_LiteMode_CustomerInsMember_SAg_Validate";

    private const string CreateNewSPName = "Acc_LiteMode_CustomerInsMember_SAg";

    public CreateMasterLiteModeService(IMessageService messageService,
        AppSettings appSettings,
        ICustomerSettingHelper helperService,
        IDynamicRepository dynamicRepository)
        : base(messageService, appSettings, helperService, dynamicRepository)
    {
    }

    public override async Task<CreateCustomerResult> Create(CreateRequest request)
    {
        var result = await ValidateSetting(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        result = await RegisterCustomer(request, CustomerLevel.Master);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.NewCustIds[0];
        result = await CreateMaster(request, newCustId);
        if (!result.IsSuccessful)
        {
            await RollbackCustomer(newCustId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var parameters = new Hashtable
        {
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
            { "site", request.Settings.GeneralInfo.Site },
            { "srecommend", request.Settings.GeneralInfo.UplineCustId },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "creator", request.Context.CreatorId },
            { "ip", request.Context.IP },
            { "username", request.Settings.GeneralInfo.UserName },
            { "settingstatus", appSettings.SettingStatus },
            { "systemid", request.Context.SystemId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(ValidateSPName, request.Context.CustDBGroupID));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
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
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()) },
            { "templatename", request.Settings.GeneralInfo.TemplateName},
            { "settingtypeall",  helperService.ObjectToXml(request.Context.UIBehavior)},
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "srecommend", request.Settings.GeneralInfo.UplineCustId },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "site", request.Settings.GeneralInfo.Site },
            { "settingstatus", appSettings.SettingStatus },
            { "remark", request.Settings.GeneralInfo.Remark }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(CreateNewSPName, request.Context.CustDBGroupID));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(newCustId, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

}