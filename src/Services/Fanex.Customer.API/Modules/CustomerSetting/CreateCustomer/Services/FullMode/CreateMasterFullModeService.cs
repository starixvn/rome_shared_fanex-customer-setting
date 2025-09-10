using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public class CreateMasterFullModeService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    public CreateMasterFullModeService(IMessageService messageService,
        AppSettings appSettings,
        ICustomerSettingHelper helperService,
        IDynamicRepository dynamicRepository)
        : base(messageService, appSettings, helperService, dynamicRepository)
    {
    }

    public override async Task<CreateCustomerResult> Create(CreateRequest request)
    {
        var result = await ValidateSetting(request);
        if (!result.IsSuccessful)
        {
            return result;
        }

        result = await RegisterCustomer(request, CustomerLevel.Master);
        if (!result.IsSuccessful)
        {
            return result;
        }

        int newCustId = result.NewCustIds[0];
        result = await CreateMaster(request, newCustId);
        if (!result.IsSuccessful)
        {
            await RollbackCustomer(newCustId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var criteria = new ValidateMasterSettingCommand(
            request.Settings.GeneralInfo.Site,
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            request.Settings.GeneralInfo.UplineCustId,
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Settings.GeneralInfo.MaxCredit,
            request.Context.IP,
            appSettings.SettingStatus,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode()),
            request.Context.SystemId
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(0, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> CreateMaster(CreateRequest request, int newCustId)
    {
        var criteria = new CreateMasterCommand(
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
            request.Settings.GeneralInfo.TransferOption,
            request.Settings.GeneralInfo.DisableAutoMPT,
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Context.SubAccId,
            request.Settings.GeneralInfo.MaxCredit,
            request.Settings.GeneralInfo.UplineCustId,
            request.Context.SystemId,
            request.Context.IP,
            request.Settings.GeneralInfo.Site,
            appSettings.SettingStatus,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, request.Context.IsFullMode())
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(new List<int> { newCustId }, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

}
