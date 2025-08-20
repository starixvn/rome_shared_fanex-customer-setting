using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;
using System.Collections;
using Fanex.Data;
using Fanex.Customer.API.Shared.Utils;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;

public interface ICreateCustomerBaseService
{
    Task<CreateCustomerResult> Create(CreateRequest request);
}

public class CreateCustomerBaseService : ICreateCustomerBaseService
{
    protected readonly IMessageService messageService;

    protected readonly AppSettings appSettings;

    protected readonly ICustomerSettingHelper helperService;

    protected readonly IDynamicRepository dynamicRepository;

    public CreateCustomerBaseService(IMessageService messageService,
        AppSettings appSettings,
        ICustomerSettingHelper helperService,
        IDynamicRepository dynamicRepository)
    {
        this.messageService = messageService;
        this.appSettings = appSettings;
        this.helperService = helperService;
        this.dynamicRepository = dynamicRepository;
    }

    public virtual Task<CreateCustomerResult> Create(CreateRequest request)
        => throw new NotImplementedException();
        
    public async Task<decimal> GetCashAccumCredit(int agentId)
    {
        var command = new GetCashAccumCredit(agentId);
        await dynamicRepository.ExecuteAsync(command);

        return command.AccumCreditReturn;
    }
    
    protected async Task RollbackCustomer(int newCustId)
    {
        const string RollbackSPName = "Acc_CustomerIns_Rollback";
        var parameters = new Hashtable
        {
            { "custid", newCustId }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RollbackSPName);
        await db.ExecuteNonQueryAsync(parameters);
    }

    protected async Task<CreateCustomerResult> RegisterCustomer(CreateRequest request, int roleId)
    {
        const string RegisterSPName = "Acc_CustomerIns_Register";
        var parameters = new Hashtable
        {
            { "username", request.Settings.GeneralInfo.UserName },
            { "sitedbgroupid", request.Context.CustDBGroupID },
            { "site", request.Settings.GeneralInfo.Site },
            { "roleid", roleId },
            { "uplineid", request.Settings.GeneralInfo.UplineCustId },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(Convert.ToInt32(outputParams["custid"]), Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }
}
