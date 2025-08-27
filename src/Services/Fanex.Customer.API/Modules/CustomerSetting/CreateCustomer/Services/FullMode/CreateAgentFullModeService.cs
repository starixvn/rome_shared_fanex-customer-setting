using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public class CreateAgentFullModeService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    public CreateAgentFullModeService(IMessageService messageService, 
        AppSettings appSettings, 
        ICustomerSettingHelper helperService,
        IDynamicRepository dynamicRepository)
        : base( messageService, appSettings, helperService, dynamicRepository)
    {
    }

    public override async Task<CreateCustomerResult> Create(CreateRequest request)
    {
        var result = await ValidateSetting(request);
        if (!result.IsSuccessful)
        {
            return result;
        }
        
        result = await RegisterCustomer(request, CustomerLevel.Agent);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.NewCustIds[0];
        result = await CreateAgent(request, newCustId);
        if (!result.IsSuccessful)
        {
            await RollbackCustomer(newCustId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var criteria = new ValidateAgentSettingCommand(
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Settings.GeneralInfo.UplineCustId,
            request.Settings.GeneralInfo.MaxCredit,
            request.Settings.GeneralInfo.MemberMaxCredit,
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            request.Context.IP,
            appSettings.SettingStatus,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()),
            request.Context.SystemId,
            request.Context.Site
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(0, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> CreateAgent(CreateRequest request, int newCustId)
    {
        var criteria = new CreateAgentCommand(
            newCustId,
            request.Settings.GeneralInfo.FirstName ?? string.Empty,
            request.Settings.GeneralInfo.LastName ?? string.Empty,
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            request.Settings.GeneralInfo.Password,
            request.Settings.GeneralInfo.Phone ?? string.Empty,
            request.Settings.GeneralInfo.MobilePhone ?? string.Empty,
            request.Settings.GeneralInfo.Fax ?? string.Empty,
            request.Settings.GeneralInfo.TransferOption,
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Context.SubAccId,
            request.Settings.GeneralInfo.MaxCredit,
            request.Settings.GeneralInfo.MemberMaxCredit,
            request.Settings.GeneralInfo.UplineCustId,
            request.Context.SystemId,
            request.Context.IP,
            request.Context.Site,
            appSettings.SettingStatus,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode())
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(newCustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

}
