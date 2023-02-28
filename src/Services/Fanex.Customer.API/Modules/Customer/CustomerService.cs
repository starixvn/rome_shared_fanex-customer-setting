using System.Data;
using Fanex.Customer.API.Modules.Customer.CopyCustomer;
using Fanex.Customer.API.Modules.Customer.PositionTaking;
using Fanex.Customer.API.Modules.Customer.SearchCustomer;
using Fanex.Customer.API.Shared.Enumerations;
using Fanex.Customer.API.Shared.Models;
using Fanex.Data;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace Fanex.Customer.API.Modules.Customer;

public interface ICustomerService
{
    Task<CustomerPT> GetCustomerPT(int custId, byte custLevel);

    Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param);
    Task<CopyMultipleCustomerResult> CopyCustomer(CopyCustomerRequest request);
}

public class CustomerService : ICustomerService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly AppSettings appSettings;
    private readonly ICopyMemberSpParamsBuilder copyMemberSpParamsBuilder;
    private readonly IThirdPartyProductService thirdPartyProductService;

    public CustomerService(
        IDynamicRepository dynamicRepository,
        IOptions<AppSettings> appSettings,
        ICopyMemberSpParamsBuilder copyMemberSpParamsBuilder,
        IThirdPartyProductService thirdPartyProductService)
    {
        this.dynamicRepository = dynamicRepository;
        this.appSettings = appSettings.Value;
        this.copyMemberSpParamsBuilder = copyMemberSpParamsBuilder;
        this.thirdPartyProductService = thirdPartyProductService;
    }

    public async Task<CustomerPT> GetCustomerPT(int custId, byte custLevel)
        => await dynamicRepository.GetAsync<CustomerPT>(new GetCustomerPTInfoCriteria(Enumeration.FromValue<UserLevel>(custLevel), custId));
        
    public async Task<IEnumerable<SuggestCustomer>> SearchCustomer(SearchCustomerRequest param)
    {
        var criteria = new SearchCustomerCriteria(param.CustId, param.CustId, param.Query, param.IsSubAccount, param.ExcludeMember, param.DirectDownLine, param.IsAccInfo, param.IsInternalAgent);
        return await dynamicRepository.FetchAsync<SuggestCustomer>(criteria);
    }

    public async Task<CopyMultipleCustomerResult> CopyCustomer(CopyCustomerRequest request)
    {
        var index = 0;
        var usernames = new List<string>();

        do
        {
            var result = request.ActorLevel.IsAgentLevel() ? await CopyMember(request) : await CopyMasterOrAgent(request);
            if (result.IsSuccessful)
            {
                index++;
                request.UserName = await GetAvailableDownlineUserName(request);

                Task.Run(async () =>
                {
                    await Sync3rdProductSettings(request, result.CustId);
                });
            }
            else
            {
                if (result.Code == 201)
                {
                    request.UserName = await GetAvailableDownlineUserName(request);
                }
                else
                {
                    return new CopyMultipleCustomerResult(usernames, result.Code, result.Message);
                }
            }

        } while (index < request.NumberOfAccounts);

        return new CopyMultipleCustomerResult(usernames);
    }

    private async Task Sync3rdProductSettings(CopyCustomerRequest param, int createdCustId)
    {
        var createdCustomer = await GetCustomerInfo(createdCustId);
        var sourceCustomer = await GetCustomerInfo(param.SourceCustId);
        var actor = await GetCustomerInfo(param.ActorId);

        if (createdCustomer != null && sourceCustomer != null && actor != null)
        {
            if (param.ActorLevel.IsAgentLevel())
            {
                await thirdPartyProductService.SyncMemberSettings(
                    createdCustomer,
                    sourceCustomer,
                    actor,
                    param);
            }
            else
            {
                await thirdPartyProductService.SyncMasterOrAgentSettings(
                    createdCustomer,
                    sourceCustomer,
                    actor,
                    param);
            }
        }
    }

    private async Task<CustomerInfo> GetCustomerInfo(int custId)
        => await dynamicRepository.GetAsync<CustomerInfo>(new GetCustomerInfoCriteria(custId, appSettings.SettingStatus));

    private async Task<CopyCustomerResult> CopyMember(CopyCustomerRequest param)
    {
        var dataTable = await GetMemberSetting(param.SourceCustId);

        if (dataTable.Rows.Count > 0)
        {
            using IObjectDb db = ObjectDbFactory.CreateInstance("Acc_CustomerInsMember_Ag");
            var spParams = copyMemberSpParamsBuilder.BuildSpParams(
                dataTable.Rows[0],
                param,
                CfsEnCode(param.Password));

            await db.ExecuteNonQueryAsync(spParams);
            var ouputParams = db.GetOutputParams();

            return new CopyCustomerResult(
                Convert.ToInt32(ouputParams["custid"]),
                Convert.ToInt32(ouputParams["errcode"]),
                ouputParams["errmsg"].ToString());
        }

        return null;
    }

    private static async Task<DataTable> GetMemberSetting(int custId)
    {
        var criteria = new GetMemberSettingCriteria(custId);
        using IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var dataSet = await db.ExecuteDataSetAsync(criteria);

        return dataSet?.Tables?.Count == 1
            ? dataSet.Tables[0]
            : new DataTable();
    }

    private async Task<CopyCustomerResult> CopyMasterOrAgent(CopyCustomerRequest request)
    {
        var criteria = BuildCopyMasterOrAgentCriteria(request);

        await dynamicRepository.ExecuteAsync(criteria);

        return new CopyCustomerResult(
            criteria.CustId,
            criteria.ErrCode,
            criteria.ErrMsg);
    }

    private CopyCustomerCommand BuildCopyMasterOrAgentCriteria(CopyCustomerRequest request)
    {
        if (request.ActorLevel.IsMasterLevel())
        {
            return new CopyAgentCommand(
                request.UserName,
                CfsEnCode(request.Password),
                request.Credit,
                request.MaxCredit,
                request.ActorId,
                request.ActorId,
                request.SourceCustId,
                request.ActorSubAccId,
                request.LanguageKey,
                request.IP,
                request.SystemId,
                appSettings.SettingStatus);
        }

        return new CopyMasterCommand(
            request.UserName,
            CfsEnCode(request.Password),
            request.Credit,
            request.ActorId,
            request.ActorId,
            request.SourceCustId,
            request.ActorSubAccId,
            request.LanguageKey,
            request.IP,
            request.SystemId,
            appSettings.SettingStatus);
    }

    public string CfsEnCode(string plainText)
    {
        var num = 30;
        var num2 = num - plainText.Length;

        if (num2 > 1)
        {
            for (var m = 1; m <= num2; m++)
            {
                plainText += Strings.Chr(0x15);
            }
        }

        var num3 = 1.0;

        for (var i = 1; i <= num; i++)
        {
            var num5 = num + Strings.Asc(Strings.Mid(plainText, i, 1)) * i;
            num3 *= num5;
        }

        plainText = num3.ToString();
        var str = string.Empty;

        for (var j = 1; j <= plainText.Length; j++)
        {
            str += CfsCode(Strings.Mid(plainText, j, 3));
        }

        var str2 = string.Empty;

        for (var k = 20; k <= str.Length - 0x12; k += 2)
        {
            str2 += Strings.Mid(str, k, 1);
        }

        return str2;
    }

    private static string CfsCode(string nWord)
    {
        var number = string.Empty;

        for (var i = 1; i <= nWord.Length; i++)
        {
            number += Strings.Asc(Strings.Mid(nWord, i, 1));
        }

        return Conversion.Hex(number);
    }

    private async Task<string> GetAvailableDownlineUserName(CopyCustomerRequest request)
    {
        var ptInfo = await GetCustomerPT(request.ActorId, request.ActorUserLevel);

        if (ptInfo != null)
        {
            var userName = request.UserName;
            var length = request.ActorLevel.IsAgentLevel()
                ? userName.Length - 3
                : userName.Length - 2;

            return $"{userName.Substring(0, length)}{ptInfo.Suffix}";
        }

        return string.Empty;
    }
}