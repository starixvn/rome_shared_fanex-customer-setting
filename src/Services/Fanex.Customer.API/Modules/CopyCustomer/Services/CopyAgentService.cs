using System.Collections;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Services;

public class CopyAgentService : CopyCustomerBaseService
{
    private const string ValidateSPName = "Acc_CustomerInsMember_CloneSetting_MAg_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CopySPName = "Acc_CustomerInsMember_CloneSetting_MAg";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public CopyAgentService(IMessageService messageService,
        AppSettings appSettings,
        IDynamicRepository dynamicRepository)
        : base(messageService, appSettings, dynamicRepository)
    {
    }

    public override async Task<CopyCustomerResult> Copy(CopyCustomerRequest request)
    {
        var result = await Validate(request);
        if (!result.IsSuccessful)
        {
            return result;
        }
        
        result = await Register(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.CustId;
        result = await CopyAgent(request, newCustId);
        if (!result.IsSuccessful)
        {
            await Rollback(newCustId);
        }

        return result;
    }

    private async Task<CopyCustomerResult> Validate(CopyCustomerRequest request)
    {
        var parameters = new Hashtable
        {
            { "credit", request.Credit },
            { "maxcredit", request.MaxCredit },
            { "mrecommend", request.UplineId },
            { "existingcustid", request.SourceCustId },
            { "language", request.LanguageKey },
            { "ip", request.IP },
            { "systemid", request.SystemId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(ValidateSPName, request.CustDBGroupID));
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
            { "sitedbgroupid", request.CustDBGroupID },
            { "site", request.Site },
            { "roleid", CustomerLevel.Agent },
            { "uplineid", request.UplineId },
            { "language", request.LanguageKey }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(Convert.ToInt32(outputParams["custid"]), Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> CopyAgent(CopyCustomerRequest request, int newCustId)
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
            { "fax", request.Fax ?? string.Empty},
            { "credit", request.Credit },
            { "maxcredit", request.MaxCredit },
            { "creator", request.ActorId },
            { "mrecommend", request.UplineId },
            { "existingcustid", request.SourceCustId },
            { "subaccid", request.ActorSubAccId },
            { "language", request.LanguageKey },
            { "ip", request.IP },
            { "systemid", request.SystemId },
            { "settingstatus", appSettings.SettingStatus },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(CopySPName, request.CustDBGroupID));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(newCustId, Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task Rollback(int newCustId)
    {
        var parameters = new Hashtable
        {
            { "custid", newCustId }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RollbackSPName);
        await db.ExecuteNonQueryAsync(parameters);
    }
}
