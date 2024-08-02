using System.Collections;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Services;

public class CopySuperService
{
    private readonly IScaleoutDBService scaleoutDBService;

    private readonly IMessageService messageService;

    private readonly AppSettings appSettings;

    private const string CheckPermissionSPName = "Acc_GetDeniedSiteCurrencyByUser";

    private const string ValidateSPName = "Acc_CustomerInsMember_CloneSetting_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CreateNewSPName = "Acc_CustomerInsMember_CloneSetting_Com";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public CopySuperService(IMessageService messageService,
        AppSettings appSettings,
        IScaleoutDBService scaleoutDBService)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.scaleoutDBService = scaleoutDBService;
    }

    public async Task<CopyCustomerResult> Copy(CopyCustomerRequest request)
    {
        return null;
    }

    private async Task<CopyCustomerResult> CheckPermission(CopyCustomerRequest request)
    {
        var parameters = new Hashtable
        {
            { "site", request.Site },
            // { "currency", request. },
            { "creator", request.ActorId },
            { "language", request.LanguageKey },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(CheckPermissionSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> Validate(CopyCustomerRequest request)
    {
        var parameters = new Hashtable
        {
            { "username", request.UserName },
            { "site", request.Site },
            { "credit", request.Credit },
            { "existingcustid", request.SourceCustId },
            { "creator", request.ActorId },
            { "language", request.LanguageKey },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(ValidateSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> Register(CopyCustomerRequest request)
    {
        var parameters = new Hashtable
        {
            { "username", request.UserName },
            { "sitedbgroupid", await scaleoutDBService.GetCustDBGroupIdBySite(request.Site) },
            { "site", request.Site },
            { "roleid", CustomerLevel.Super },
            { "language", request.LanguageKey }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(ValidateSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> CopySuper(CopyCustomerRequest request)
    {
        var parameters = new Hashtable
        {
            { "username", request.UserName },
            { "sitedbgroupid", await scaleoutDBService.GetCustDBGroupIdBySite(request.Site) },
            { "site", request.Site },
            { "roleid", CustomerLevel.Super },
            { "language", request.LanguageKey }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(ValidateSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }
}
