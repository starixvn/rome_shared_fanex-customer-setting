using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;
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
        var criteria = new RollbackRegisterCustomer(newCustId);
        await dynamicRepository.ExecuteAsync(criteria);
    }

    protected async Task<CreateCustomerResult> RegisterCustomer(CreateRequest request, int roleId)
    {
        var criteria = new RegisterCustomerBeforeAddNew(
            request.Settings.GeneralInfo.UserName,
            request.Context.CustDBGroupID,
            request.Settings.GeneralInfo.Site,
            (byte)roleId,
            request.Settings.GeneralInfo.UplineCustId,
            LanguageUtils.ToLanguageKey(request.Context.Language)
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(0, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(criteria.CustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }
}
