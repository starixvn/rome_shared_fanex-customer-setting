using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public class CreateSuperFullModeService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    public CreateSuperFullModeService(IMessageService messageService, 
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
        
        result = await RegisterSuperCustomer(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.NewCustIds[0];
        result = await CreateSuper(request, newCustId);
        if (!result.IsSuccessful)
        {
            await RollbackCustomer(newCustId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var criteria = new ValidateSuperSettingCommand(
            request.Settings.GeneralInfo.Currency,
            request.Settings.GeneralInfo.Site,
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Settings.GeneralInfo.MaxCredit,
            appSettings.SettingStatus,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode())
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(0, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> RegisterSuperCustomer(CreateRequest request)
    {
        var criteria = new RegisterSuperCustomerCommand(
            request.Settings.GeneralInfo.UserName,
            request.Context.CustDBGroupID,
            request.Context.Site,
            request.Settings.GeneralInfo.Currency,
            CustomerLevel.Super,
            LanguageUtils.ToLanguageKey(request.Context.Language)
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(criteria.CustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> CreateSuper(CreateRequest request, int newCustId)
    {
        var criteria = new CreateSuperCommand(
            newCustId,
            request.Settings.GeneralInfo.FirstName ?? string.Empty,
            request.Settings.GeneralInfo.LastName ?? string.Empty,
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            request.Settings.GeneralInfo.Password,
            request.Settings.GeneralInfo.Phone ?? string.Empty,
            request.Settings.GeneralInfo.MobilePhone ?? string.Empty,
            request.Settings.GeneralInfo.Fax ?? string.Empty,
            request.Settings.GeneralInfo.Remark ?? string.Empty,
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Settings.GeneralInfo.MaxCredit,
            request.Context.SystemId,
            request.Context.IP,
            appSettings.SettingStatus,
            request.Settings.GeneralInfo.Site,
            request.Settings.GeneralInfo.Currency,
            request.Settings.GeneralInfo.IsInternal,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode())
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(newCustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

}
