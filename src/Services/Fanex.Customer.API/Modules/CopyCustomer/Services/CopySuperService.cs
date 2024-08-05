using System.Collections;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Services;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Services;

public class CopySuperService : CopyCustomerBaseService
{
    private const string CheckPermissionSPName = "Acc_GetDeniedSiteCurrencyByUser";

    private const string ValidateSPName = "Acc_CustomerInsMember_CloneSetting_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CopySPName = "Acc_CustomerInsMember_CloneSetting_Com";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public CopySuperService(IMessageService messageService,
        AppSettings appSettings,
        IScaleoutDBService scaleoutDBService)
        : base(messageService, appSettings, scaleoutDBService)
    {
    }

    public override async Task<CopyCustomerResult> Copy(CopyCustomerRequest request)
    {
        var siteDBGroupId = await scaleoutDBService.GetCustDBGroupIdBySite(request.Site);
        var result = await CheckPermission(request, siteDBGroupId);
        if (!result.IsSuccessful)
        {
            return result;
        }
        
        result = await Validate(request, siteDBGroupId);
        if (!result.IsSuccessful)
        {
            return result;
        }
        
        result = await Register(request, siteDBGroupId);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.CustId;
        result = await CopySuper(request, newCustId, siteDBGroupId);
        if (!result.IsSuccessful)
        {
            Rollback(newCustId);
        }

        return result;
    }

    private async Task<CopyCustomerResult> CheckPermission(CopyCustomerRequest request, int siteDBGroupId)
    {
        var parameters = new Hashtable
        {
            { "site", request.Site },
            { "creator", request.ActorId },
            { "language", request.LanguageKey },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(scaleoutDBService.GetSPNameWithPrefix(CheckPermissionSPName, siteDBGroupId));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> Validate(CopyCustomerRequest request, int siteDBGroupId)
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

        using IObjectDb db = ObjectDbFactory.CreateInstance(scaleoutDBService.GetSPNameWithPrefix(ValidateSPName, siteDBGroupId));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> Register(CopyCustomerRequest request, int siteDBGroupId)
    {
        var parameters = new Hashtable
        {
            { "username", request.UserName },
            { "sitedbgroupid", siteDBGroupId },
            { "site", request.Site },
            { "roleid", CustomerLevel.Super },
            { "language", request.LanguageKey }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(Convert.ToInt32(outputParams["custid"]), Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> CopySuper(CopyCustomerRequest request, int newCustId, int siteDBGroupId)
    {
        var parameters = new Hashtable
        {
            { "custid", newCustId },
            { "username", request.UserName },
            { "userpwd", request.Password },
            { "firstname", request.FirstName ?? string.Empty },
            { "lastname", request.LastName ?? string.Empty},
            { "phone", request.Phone ?? string.Empty},
            { "mobilephone", request.MobilePhone ?? string.Empty},
            { "site", request.Site },
            { "fax", request.Fax ?? string.Empty},
            { "credit", request.Credit },
            { "isinternal", request.IsInternal },
            { "existingcustid", request.SourceCustId },
            { "creator", request.ActorId },
            { "language", request.LanguageKey },
            { "systemid", request.SystemId },
            { "ip", request.IP },
            { "settingstatus", appSettings.SettingStatus },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(scaleoutDBService.GetSPNameWithPrefix(CopySPName, siteDBGroupId));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(newCustId, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
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
