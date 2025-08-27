using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria.Cash;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CustomerSetting.CreateCustomer.Criteria;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.Cash;

public class CreateCashMemberService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    private readonly ICustomerService _customerService;

    private List<int> newCustIds = new List<int>();

    private List<string> newUserNames = new List<string>();

    public CreateCashMemberService(IMessageService messageService,
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
                var suggestNewUserNameResult = await _customerService.GetSuggestUsername(
                    new Customer.Requests.GetSuggestUsernameRequest()
                    { 
                        ActorName = request.Settings.GeneralInfo.UplineUserName, 
                        RoleId = request.Settings.GeneralInfo.UplineRoleId
                    });

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

            result = await Register(request);
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
                await Rollback(newCustId);
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
        var criteria = new ValidateCashMemberSetting(
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, false),
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Settings.GeneralInfo.UplineCustId,
            request.Context.SystemId,
            request.Context.IP,
            appSettings.SettingStatus
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.MsgType, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(0, criteria.ErrCode, msg.MainMessage, msg.SubMessages, newUserNames);
    }

    private async Task<CreateCustomerResult> Register(CreateRequest request)
    {
        var criteria = new RegisterCustomerBeforeAddNew(
            request.Settings.GeneralInfo.UserName,
            request.Context.CustDBGroupID,
            request.Settings.GeneralInfo.Site,
            CustomerLevel.Member,
            request.Settings.GeneralInfo.UplineCustId,
            LanguageUtils.ToLanguageKey(request.Context.Language)
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(0, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(criteria.CustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

    private async Task<CreateCustomerResult> CreateMember(CreateRequest request, int newCustId)
    {
        var criteria = new CreateCashMemberCommand(
            newCustId,
            request.Settings.GeneralInfo.FirstName ?? string.Empty,
            request.Settings.GeneralInfo.LastName ?? string.Empty,
            request.Context.CreatorId,
            request.Settings.GeneralInfo.UserName,
            request.Settings.GeneralInfo.Password,
            request.Settings.GeneralInfo.Phone ?? string.Empty,
            request.Settings.GeneralInfo.MobilePhone ?? string.Empty,
            request.Settings.GeneralInfo.Remark ?? string.Empty,
            request.Settings.GeneralInfo.Bank?.BankId ?? 0,
            request.Settings.GeneralInfo.Bank?.AccountNumber ?? string.Empty,
            helperService.CreateSettingTypesJson(request.Settings.ProductGroups, false),
            LanguageUtils.ToLanguageKey(request.Context.Language),
            request.Context.SubAccId,
            request.Settings.GeneralInfo.UplineCustId,
            request.Context.SystemId,
            request.Context.IP,
            appSettings.SettingStatus
        );

        await dynamicRepository.ExecuteAsync(criteria);
        var msg = messageService.TranslateMessage(criteria.MsgType, criteria.ErrMsg, request.Context.Language);
        return new CreateCustomerResult(newCustId, criteria.ErrCode, msg.MainMessage, msg.SubMessages);
    }

    private async Task Rollback(int newCustId)
    {
        var criteria = new RollbackRegisterCustomer(newCustId);
        await dynamicRepository.ExecuteAsync(criteria);
    }
}
