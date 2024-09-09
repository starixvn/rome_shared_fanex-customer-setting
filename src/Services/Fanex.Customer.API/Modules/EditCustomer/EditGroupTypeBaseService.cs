using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;
using Fanex.Customer.API.Modules.EditCustomer.EditGroupTypeServices;
using Fanex.Customer.API.Modules.EditCustomer.Requests;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Services;

namespace Fanex.Customer.API.Modules.EditCustomer;

public class EditGroupTypeBaseService: IEditGroupTypeBaseService
{
    protected readonly IMessageService messageService;

    protected readonly ICacheRefService cacheRefService;

    public EditGroupTypeBaseService(IMessageService messageService, ICacheRefService cacheRefService)
    {
        this.messageService = messageService;
        this.cacheRefService = cacheRefService;
    }

    public virtual UpdateGroupTypeResult Validate(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        throw new NotImplementedException();
    }

    public virtual Task<UpdateGroupTypeResult> Update(
        EditRequestContext context,
        int productGroupId,
        int groupTypeId,
        IEnumerable<GroupItemDefault> groupItems,
        byte settingStatus)
    {
        throw new NotImplementedException();
    }
}
