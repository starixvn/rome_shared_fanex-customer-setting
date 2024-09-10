using System.Collections;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Customer.API.Modules.CreateCustomer.Criteria;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Resources;
using System.Text.RegularExpressions;
using Fanex.Customer.API.Modules.CreateCustomer.Requests;

namespace Fanex.Customer.API.Modules.CreateCustomer.Services;
public interface ICreateNickNameService
{
    bool ValidateRequest(string nickName);

    Task<CustomerSettingResult> Create(CreateNickNameRequest request);
}

public class CreateNickNameService : ICreateNickNameService
{
    protected readonly IDynamicRepository dynamicRepository;
    public CreateNickNameService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public bool ValidateRequest(string nickName)
    {
        Regex NicknameRule = new Regex(@"^[a-zA-Z0-9]{5,20}$");

        return !string.IsNullOrEmpty(nickName) || NicknameRule.IsMatch(nickName);
    }

    public async Task<CustomerSettingResult> Create(CreateNickNameRequest request)
    {
        if(request.IsEnableUpdateNickname)
        {
            var result = await Register(request);
            if (!result.IsSuccessful)
            {
                return result;
            }

            result = await DoProcessing(request);
            if (!result.IsSuccessful)
            {
                Rollback(request.CustId);
            }

            return result;
        }
        
        return await UpdateLoginNickNameOnlyMode(request);
    }

    private async Task<CreateCustomerResult> Register(CreateNickNameRequest request)
    {
        var parameters = new Hashtable
        {
            { "nickname", request.NickName },
            { "language", LanguageUtils.ToLanguageKey(request.Language) },
            { "custid", request.CustId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance("Age_CustomerInsNickName_Register");
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();

        return new CreateCustomerResult(request.CustId, Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]));
    }

    private async Task<CreateCustomerResult> DoProcessing(CreateNickNameRequest request)
    {
        var result = await CreateNickName(request);
        if (result.IsSuccessful)
        {
            return await UpdateLoginNickNameOnlyMode(request);
        }

        return result;
    }

    private async void Rollback(int custId)
    {
        var parameters = new Hashtable
        {
            { "custid", custId }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance("Age_CustomerInsNickName_Rollback");
        await db.ExecuteNonQueryAsync(parameters);
    }

    private async Task<CreateCustomerResult> CreateNickName(CreateNickNameRequest request)
    {
        var command = new CreateNicknameCommand(request.CustId, request.NickName, LanguageUtils.ToLanguageKey(request.Language), request.Ip);
        await dynamicRepository.ExecuteAsync(command);
        var errMessage = command.ErrCode == 0 ? StaticResources.Get("ConfiguredNicknameSuccessfully", request.Language) : command.ErrMsg;

        return new CreateCustomerResult(request.CustId, command.ErrCode, errMessage);
    }

    private async Task<CreateCustomerResult> UpdateLoginNickNameOnlyMode(CreateNickNameRequest request)
    {
        var command = new UpdateNicknameLoginOnlyCommand(request.CustId, LanguageUtils.ToLanguageKey(request.Language), request.Ip);
        await dynamicRepository.ExecuteAsync(command);
        var errMessage = command.ErrCode == 0 ? StaticResources.Get("ConfiguredNicknameSuccessfully", request.Language) : command.ErrMsg;

        return new CreateCustomerResult(request.CustId, command.ErrCode, errMessage);
    }
}