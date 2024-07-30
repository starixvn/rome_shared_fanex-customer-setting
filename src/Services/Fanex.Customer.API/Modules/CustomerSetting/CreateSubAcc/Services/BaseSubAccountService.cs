using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CreateSubAcc.Requests;

namespace Fanex.Customer.API.Modules.CreateSubAcc.Services;
public class BaseSubAccountService
{
    public BaseSubAccountService(IMessageService messageService, IScaleoutDBService scaleoutDBService)
    {
        this.scaleoutDBService = scaleoutDBService;
        this.messageService = messageService;
    }
    protected readonly IScaleoutDBService scaleoutDBService;
    protected readonly IMessageService messageService;

    public virtual async Task<CustomerSettingResult> Create(CreateSubAccRequest request)
    {
        var result = await Register(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newId = result.NewCustId;
        result = await CreateAgent(request, newId);
        if (!result.IsSuccessful)
        {
            Rollback(request.CustId, newId);
        }

        return result;
    }

    protected virtual Task<CreateCustomerResult> Register(CreateSubAccRequest request) => throw new NotImplementedException();

    protected virtual Task<CreateCustomerResult> CreateAgent(CreateSubAccRequest request, int subAccId) => throw new NotImplementedException();

    protected virtual void Rollback(int custId, int subAccId) => throw new NotImplementedException();
}