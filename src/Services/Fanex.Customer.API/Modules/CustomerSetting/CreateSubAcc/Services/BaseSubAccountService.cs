using System.Collections;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CreateSubAcc.Requests;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.CreateSubAcc.Services;
public class BaseSubAccountService
{
    private const string RegisterSPName = "Age_CustSubIns_Register";
    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public BaseSubAccountService(IMessageService messageService, IDynamicRepository dynamicRepository)
    {
        this.messageService = messageService;
        this.dynamicRepository = dynamicRepository;
    }

    protected readonly IMessageService messageService;
    protected readonly IDynamicRepository dynamicRepository;

    protected virtual int IsResetNickName => throw new NotImplementedException();

    public virtual async Task<CustomerSettingResult> Create(BaseSubAccountRequest request)
    {
        var result = await Register(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newId = result.NewCustId;
        result = await DoProcessing(request, newId);
        if (!result.IsSuccessful)
        {
            Rollback(request.CustId, newId);
        }

        return result;
    }

    protected virtual async Task<CreateCustomerResult> Register(BaseSubAccountRequest request)
    {
        var parameters = new Hashtable
        {
            { "username", request.UserName },
            { "nickname", request.NickName },
            { "sitedbgroupid", request.CustDBGroupID },
            { "language", LanguageUtils.ToLanguageKey(request.Language) },
            { "custid", request.CustId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        
        return new CreateCustomerResult(Convert.ToInt32(outputParams["subaccid"]), Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]));
    }

    protected virtual Task<CreateCustomerResult> DoProcessing(BaseSubAccountRequest request, int subAccId) => throw new NotImplementedException();

    protected virtual async void Rollback(int custId, int subAccId)
    {
        var parameters = new Hashtable
        {
            { "isResetNickname", IsResetNickName },
            { "custid", custId },
            { "SubAccId", subAccId }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RollbackSPName);
        await db.ExecuteNonQueryAsync(parameters);
    }
}