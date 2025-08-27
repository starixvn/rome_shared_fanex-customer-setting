using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public class CreateDirectMemberService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    private readonly ICustomerService _customerService;

    private List<int> newCustIds = new List<int>();

    private List<string> newUserNames = new List<string>();

    public CreateDirectMemberService(IMessageService messageService,
        AppSettings appSettings,
        ICustomerSettingHelper helperService,
        ICustomerService customerService,
        IDynamicRepository dynamicRepository)
        : base(messageService, appSettings, helperService, dynamicRepository)
    {
        this._customerService = customerService;
    }

    public override async Task<CreateCustomerResult> Create(CreateRequest request)
    {
        int retryCount = 0;
        for (var i = 0; i < request.Context.NoOfCreateAccount.GetValueOrDefault(); i++)
        {
            var result = await ValidateSetting(request);
            if (!result.IsSuccessful)
            {
                result.NewUserNames = newUserNames;
                return result;
            }

            if (i > 0 || retryCount > 0)
            {
                var suggestNewUserNameResult = await _customerService.GetSuggestDirectMemberUsername(request.Settings.GeneralInfo.UplineUserName);
                if (!suggestNewUserNameResult.IsSuccessful)
                {
                    return new CreateCustomerResult(0,
                        code: suggestNewUserNameResult.Code,
                        message: suggestNewUserNameResult.Message,
                        newUserNames: newUserNames);
                }
                else
                {
                    request.Settings.GeneralInfo.UserName = request.Settings.GeneralInfo.UplineUserName + suggestNewUserNameResult.CustomData;
                }
            }

            result = await RegisterCustomer(request, CustomerLevel.Member);
            if (!result.IsSuccessful)
            {
                if (result.Code == 201 && retryCount <= 5)
                {
                    retryCount++;
                    i--;
                    continue;
                }
                else
                {
                    return result;
                }
            }
            else
            {
                retryCount = 0;
            }

            int newCustId = result.NewCustIds[0];
            result = await CreateMember(request, newCustId);
            if (!result.IsSuccessful)
            {
                await RollbackCustomer(newCustId);
                return result;
            }
            else
            {
                newUserNames.Add(request.Settings.GeneralInfo.UserName);
                newCustIds.Add(newCustId);
            }
        }

        return new CreateCustomerResult(
            newCustIds: newCustIds,
            newUserNames: newUserNames);
    }

    private async Task<CreateCustomerResult> ValidateSetting(CreateRequest request)
    {
        var criteria = new ValidateDirectMemberSettingCommand(
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            request.Settings.GeneralInfo.MaxCredit,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, false),
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Settings.GeneralInfo.UplineCustId,
            request.Settings.Group,
            request.Context.SystemId,
            request.Context.IP
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(0, criteria.ErrCode, msg.MainMessage, msg.SubMessages, newUserNames);
    }

    private async Task<CreateCustomerResult> CreateMember(CreateRequest request, int newCustId)
    {
        var criteria = new CreateDirectMemberCommand(
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
            request.Settings.GeneralInfo.MaxCredit,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, false),
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Context.SubAccId,
            request.Settings.GeneralInfo.UplineCustId,
            request.Settings.Group,
            request.Context.SystemId,
            request.Context.IP,
            appSettings.SettingStatus
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.Msgtype, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(newCustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }
}
