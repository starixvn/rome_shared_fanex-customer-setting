using System.Collections;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Helpers;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Customer.API.Shared.Constants;
using Fanex.Customer.API.Modules.Customer;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services.FullMode;

public class CreateDirectMemberService : CreateCustomerBaseService, ICreateCustomerBaseService
{
    private readonly ICustomerService _customerService;

    private const string ValidateSPName = "Acc_CustomerInsMember_DirectMember_Validate";

    private const string CreateNewSPName = "Acc_CustomerInsMember_DirectMember";

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
        var parameters = new Hashtable
        {
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, false) },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "uplineid", request.Settings.GeneralInfo.UplineCustId },
            { "ugroup", request.Settings.Group },
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(ValidateSPName, request.Context.CustDBGroupID));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(0, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages, newUserNames);
    }

    private async Task<CreateCustomerResult> CreateMember(CreateRequest request, int newCustId)
    {
        var parameters = new Hashtable
        {
            { "custid", newCustId },
            { "firstname", request.Settings.GeneralInfo.FirstName ?? string.Empty },
            { "lastname", request.Settings.GeneralInfo.LastName ?? string.Empty},
            { "creator", request.Context.CreatorId },
            { "username", request.Settings.GeneralInfo.UserName },
            { "userpwd", request.Settings.GeneralInfo.Password },
            { "phone", request.Settings.GeneralInfo.Phone ?? string.Empty},
            { "mobilephone", request.Settings.GeneralInfo.MobilePhone ?? string.Empty},
            { "fax", request.Settings.GeneralInfo.Fax ?? string.Empty},
            { "credit", request.Settings.GeneralInfo.MaxCredit },
            { "settingxml", helperService.CreateSettingTypesJson(request.Settings.ProductGroups, false) },
            { "language", LanguageUtils.ToLanguageKey(request.Context.Language) },
            { "subaccid", request.Context.SubAccId },
            { "uplineid", request.Settings.GeneralInfo.UplineCustId },
            { "ugroup", request.Settings.Group},
            { "systemid", request.Context.SystemId },
            { "ip", request.Context.IP },
            { "settingstatus", appSettings.SettingStatus },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(SPNameUtils.GetSPNameWithPrefix(CreateNewSPName, request.Context.CustDBGroupID));
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["msgtype"]), Convert.ToString(outputParams["errmsg"]), request.Context.Language);
        return new CreateCustomerResult(newCustId, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }
}
