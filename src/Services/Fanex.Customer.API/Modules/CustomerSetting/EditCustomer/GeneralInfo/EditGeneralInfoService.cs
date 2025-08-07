using System.Collections;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Shared.Models;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;

namespace Fanex.Customer.API.Modules.EditCustomer.Services.EditGroupTypeServices;

public class GeneralInfoService
{
    private readonly ISiteDBGroupService siteDBGroupService;

    public GeneralInfoService(ISiteDBGroupService siteDBGroupService)
    {
        this.siteDBGroupService = siteDBGroupService;
    }

    public string GetSPName() => "Acc_CustomerUpdGeneralInfo";

    public async Task<Hashtable> GenerateSPParams(EditRequest request, byte SettingStatus = 0)
    {
        var spParams = new Hashtable {
            { "custid", request.Context.CustId },
            { "firstname", request.Settings.GeneralInfo.FirstName ?? string.Empty },
            { "lastname", request.Settings.GeneralInfo.LastName ?? string.Empty},
            { "phone", request.Settings.GeneralInfo.Phone ?? string.Empty},
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone ?? string.Empty},
            { "fax", request.Settings.GeneralInfo.Fax ?? string.Empty},
            { "subaccid", request.Context.SubAccId },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "ip", request.Context.IP },
            { "userid", request.Context.ActorId },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
        };

        if (request.Context.RoleId == (int)CustomerLevel.Agent)
        {
            spParams.Add("maxcredit", request.Settings.GeneralInfo.MemberMaxCredit);
            spParams.Add("accumcreditallcashmember", await GetTotalCashMemberReturnCredit(request.Context.CustId));
        }

        if (request.Context.RoleId == (int)CustomerLevel.Member)
        {
            spParams.Add("totalamt", await GetTotalAmountMember(request.Context.CustId, request.Settings.GeneralInfo.Site));
            spParams.Add("accumcreditallcashmember", await GetTotalCashMemberReturnCredit(request.Settings.GeneralInfo.UplineCustId));
        }

        return spParams;
    }

    public CustomerSettingResult Validate(EditMultipleRequest request)
    {
        return (CustomerSettingResult)Result.CreateSuccess();
    }

    private async Task<decimal> GetTotalAmountMember(int custId, string site)
    {
        var siteDBName = await siteDBGroupService.GetSiteDBName(site);
        using IObjectDb db = ObjectDbFactory.CreateInstance($"{siteDBName}_Acc_CustomerTotalAmountSel");
        await db.ExecuteNonQueryAsync(new { CustId = custId });
        return (decimal)db.GetParameterValue("totalamt");
    }

    private async Task<decimal> GetTotalCashMemberReturnCredit(int agentId)
    {
        using IObjectDb db = ObjectDbFactory.CreateInstance($"Age_CashAccumCredit_AgSel");
        await db.ExecuteNonQueryAsync(new { CustId = agentId });
        return (decimal)db.GetParameterValue("accumcreditreturn");
    }
}
