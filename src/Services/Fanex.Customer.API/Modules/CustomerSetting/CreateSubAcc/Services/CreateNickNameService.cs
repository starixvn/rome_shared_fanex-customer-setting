using System.Collections;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CreateSubAcc.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Resources;
using System.Text.RegularExpressions;

namespace Fanex.Customer.API.Modules.CreateSubAcc.Services;
public interface ICreateNickNameSubAccountService
{
    bool ValidateRequest(BaseSubAccountRequest request);

    Task<CustomerSettingResult> Create(BaseSubAccountRequest request);
}

public class CreateNickNameSubAccountService : BaseSubAccountService, ICreateNickNameSubAccountService
{
    public CreateNickNameSubAccountService(IMessageService messageService, IScaleoutDBService scaleoutDBService, IDynamicRepository dynamicRepository)
        : base(messageService, scaleoutDBService, dynamicRepository)
    {
    }

    public bool ValidateRequest(BaseSubAccountRequest request)
    {
        Regex NicknameRule = new Regex(@"^[a-zA-Z0-9]{5,20}$");

        return NicknameRule.IsMatch(request.NickName);
    }

    protected override int IsResetNickName => 1;

    protected override async Task<CreateCustomerResult> Register(BaseSubAccountRequest request)
    {
        var nickNameRequest = (CreateNickNameRequest)request;
        var parameters = new Hashtable
        {
            { "subaccid", nickNameRequest.SubAccId },
            { "nickname", request.NickName },
            { "sitedbgroupid", await scaleoutDBService.GetCustDBGroupIdBySite(request.Site) },
            { "language", LanguageUtils.ToLanguageKey(request.Language) },
            { "custid", request.CustId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance("Age_CustSubIns_Register");
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();

        return new CreateCustomerResult(nickNameRequest.SubAccId, Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]));
    }

    protected override async Task<CreateCustomerResult> DoProcessing(BaseSubAccountRequest request, int subAccId)
    {
        var result = await CreateNickName(request, subAccId);
        if (result.IsSuccessful)
        {
            return await UpdateLoginNickNameOnlyMode(request, subAccId);
        }

        return result;
    }

    private async Task<CreateCustomerResult> CreateNickName(BaseSubAccountRequest request, int subAccId)
    {
        var command = new CreateSubAccountNicknameCommand(request.CustId, request.NickName, LanguageUtils.ToLanguageKey(request.Language), request.IP);
        await dynamicRepository.ExecuteAsync(command);
        var errMsg = command.ErrCode == 0 ? StaticResources.Get("ConfiguredNicknameSuccessfully", request.Language) : command.ErrMsg;

        return new CreateCustomerResult(subAccId, command.ErrCode, errMsg);
    }

    private async Task<CreateCustomerResult> UpdateLoginNickNameOnlyMode(BaseSubAccountRequest request, int subAccId)
    {
        var command = new UpdateSubAccountNicknameLoginOnlyCommand(request.CustId, subAccId, LanguageUtils.ToLanguageKey(request.Language), request.IP);
        await dynamicRepository.ExecuteAsync(command);
        var errMsg = command.ErrCode == 0 ? StaticResources.Get("ConfiguredNicknameSuccessfully", request.Language) : command.ErrMsg;

        return new CreateCustomerResult(subAccId, command.ErrCode, errMsg);
    }
}