using System.Collections;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CreateSubAcc.Requests;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Modules.EditCustomer.EndPoints;

namespace Fanex.Customer.API.Modules.CreateSubAcc.Services;
public class BaseSubAccountService
{
    private const string RegisterSPName = "Age_CustSubIns_Register";
    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public BaseSubAccountService(IMessageService messageService, IScaleoutDBService scaleoutDBService, IDynamicRepository dynamicRepository)
    {
        this.scaleoutDBService = scaleoutDBService;
        this.messageService = messageService;
        this.dynamicRepository = dynamicRepository;
    }
    protected readonly IScaleoutDBService scaleoutDBService;
    protected readonly IMessageService messageService;
    protected readonly IDynamicRepository dynamicRepository;

    protected virtual int IsResetNickName => throw new NotImplementedException();

    public virtual async Task<CustomerSettingResult> Create(BaseSubAccountRequest request)
    {
        Console.WriteLine("processing...");
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
            { "sitedbgroupid", await scaleoutDBService.GetCustDBGroupIdBySite(request.Site) },
            { "language", LanguageUtils.ToLanguageKey(request.Language) },
            { "custid", request.CustId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var message = messageService.TranslateMessage(Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]), request.Language);

        return new CreateCustomerResult(Convert.ToInt32(outputParams["subaccid"]), Convert.ToInt32(outputParams["errcode"]), message.MainMessage, message.SubMessages);
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