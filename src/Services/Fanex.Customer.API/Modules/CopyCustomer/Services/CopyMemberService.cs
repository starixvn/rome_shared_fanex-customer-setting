using System.Collections;
using Fanex.Customer.API.Modules.CopyCustomer.Models;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Services;

public class CopyMemberService : CopyCustomerBaseService
{
    private const string ValidateSPName = "Acc_CustomerInsMember_Ag_Validate";

    private const string RegisterSPName = "Acc_CustomerIns_Register";

    private const string CopySPName = "Acc_CustomerInsMember_Ag";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public CopyMemberService(IMessageService messageService,
        AppSettings appSettings)
        : base(messageService, appSettings)
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
        result = await CopyMember(request, newCustId);
        if (!result.IsSuccessful)
        {
            Rollback(newCustId);
        }

        return result;
    }

    private async Task<CopyCustomerResult> Validate(CopyCustomerRequest request)
    {
        var parameters = new Hashtable
        {
            { "creator", request.ActorId },
            { "username", request.UserName },
            { "credit", request.Credit },
            { "language", request.LanguageKey },
            { "recommend", request.UplineId },
            { "systemid", request.SystemId },
            { "ip", request.IP },
            { "issearchandcopy", true },
            { "existingcustid", request.SourceCustId },
            { "isqrcode", request.IsQrCode },
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
            { "roleid", CustomerLevel.Member },
            { "uplineid", request.UplineId },
            { "language", request.LanguageKey }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Language);
        return new CopyCustomerResult(Convert.ToInt32(outputParams["custid"]), Convert.ToInt32(outputParams["errcode"]), 0, msg.MainMessage);
    }

    private async Task<CopyCustomerResult> CopyMember(CopyCustomerRequest request, int newCustId)
    {
        var parameters = new Hashtable
        {
            { "custid", newCustId },
            { "firstname", request.FirstName ?? string.Empty },
            { "lastname", request.LastName ?? string.Empty},
            { "creator", request.ActorId },
            { "username", request.UserName },
            { "userpwd", request.Password },
            { "phone", request.Phone ?? string.Empty},
            { "mobilephone", request.MobilePhone ?? string.Empty},
            { "fax", request.Fax ?? string.Empty},
            { "credit", request.Credit },
            { "language", request.LanguageKey },
            { "subaccid", request.ActorSubAccId },
            { "recommend", request.UplineId },
            { "systemid", request.SystemId },
            { "ip", request.IP },
            { "issearchandcopy", true },
            { "existingcustid", request.SourceCustId },
            { "isqrcode", request.IsQrCode },
            { "settingstatus", appSettings.SettingStatus },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(CopySPName, request.CustDBGroupID));
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
