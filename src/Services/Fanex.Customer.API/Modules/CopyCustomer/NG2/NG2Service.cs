using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Fanex.Customer.API.Modules.Customer.Models;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using ONEbook.NG2.API.Client.Configurations;
using ONEbook.NG2.API.Client.Models;
using ONEbook.NG2.API.Client.Services;
using ONEbook.UM.ThirdParty.AspNetCore.Services;

namespace Fanex.Customer.API.Modules.CopyCustomer.NG2;

public interface ING2Service
{
    Task SyncCustomerSettings(
        CustomerInfo createdCustomer,
        bool disableNG2,
        CustomerInfo actor,
        CopyCustomerRequest request);
}

public class NG2Service : ING2Service
{
    private readonly Logging.ILogger logger;
    private readonly IUMService thirdPartyUMService;
    private readonly NG2Configuration ng2Configuration;
    private readonly ING2StatusService ng2StatusService;
    private readonly IDynamicRepository dynamicRepository;

    public NG2Service(
        Logging.ILogger logger,
        IUMService thirdPartyUMService,
        NG2Configuration ng2Configuration,
        ING2StatusService ng2StatusService,
        IDynamicRepository dynamicRepository)
    {
        this.logger = logger;
        this.thirdPartyUMService = thirdPartyUMService;
        this.ng2Configuration = ng2Configuration;
        this.ng2StatusService = ng2StatusService;
        this.dynamicRepository = dynamicRepository;
    }

    public async Task SyncCustomerSettings(
        CustomerInfo createdCustomer,
        bool disableNG2,
        CustomerInfo actor,
        CopyCustomerRequest request)
    {
        if (!actor.HideNG2 && actor.SyncedNG2)
        {
            try
            {
                await AutoInsertAccount(
                    createdCustomer.CustId,
                    createdCustomer.RoleId,
                    actor.CustId,
                    disableNG2,
                    actor.Currency,
                    actor.CustId,
                    request.SystemId,
                    request.IP,
                    request.Language,
                    request.ActorSubAccId);
            }
            catch (Exception ex)
            {
                await logger.WarnAsync(ex.ToString());
            }
        }
    }

    public async Task AutoInsertAccount(
        int custId,
        byte roleId,
        int uplineId,
        bool uplineDisabled,
        int currencyId,
        int userId,
        byte systemId,
        string ip,
        string language = "en-US",
        int subAccId = 0)
    {
        var auth = new Auth(ng2Configuration.Login, ng2Configuration.Password, SiteId.Alpha);
        var call = new Call()
        {
            Level = roleId,
            UplineId = uplineId,
            CurrencyId = currencyId,
            IsDisabled = uplineDisabled
        };
        call.AccountArray.Add(new AccountInfo { ObCustId = custId });
        var request = new Request(auth, call)
        {
            UserId = userId,
            Language = language
        };

        var function = APIFunc.CreateAccount.ToString();
        var logId = await LogRequest(new LogAPIRequest(
            request.Auth.SiteId == SiteId.NAP ? 3 : 2,
            ProductIds.NG2,
            request.UserId,
            function,
            request.Call.AccountArray[0].ObCustId,
            FunctionToId(function),
            JsonToXml(JsonConvert.SerializeObject(request))
        ));
        var response = await SyncCustomers(request);
        await LogResponse(logId, response.Result.ErrorCode, JsonConvert.SerializeObject(response));

        if (response.Result.IsSuccess && (response.Result.AccountArray.Count <= 0 || response.Result.AccountArray[0].IsSuccess))
        {
            var result = await SyncStatus(custId, userId, ip, subAccId);

            if (result.IsSuccessful)
            {
                await EnableStatus(custId, userId, uplineDisabled, ip, systemId, subAccId);
            }
        }
    }

    private async Task<Response> SyncCustomers(Request request)
    {
        var response = new Response();

        var umInfo = await thirdPartyUMService.GetUnderMaintenanceTime(ProductIds.NG2);
        if (umInfo != null && umInfo.IsUnderMaintenanceTime)
        {
            response.Result.ErrorCode = 5003;
            //response.Result.ErrorDescription = string.Format(StaticResources.Get("UM", request.Language), umInfo.From, umInfo.To);
            return response;
        }

        return await ng2StatusService.SyncCustomers(request);
    }

    private async Task<Shared.Models.Result> SyncStatus(int custId, int userId, string ip, int subAccId)
    {
        var command = new SyncStatusCommand(custId, userId, ip, subAccId);
        await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        return new Shared.Models.Result(command.ErrCode, command.ErrMsg);
    }

    private async Task<Shared.Models.Result> EnableStatus(int custId, int userId, bool uplineDisabled, string ip, byte systemId, int subAccId)
    {
        var command = new EnableStatusCommand(custId, userId, uplineDisabled, ip, systemId, subAccId);
        await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        return new Shared.Models.Result(command.ErrCode, command.ErrMsg);
    }

    private static int FunctionToId(string functionName)
    {
        int length = functionName.Length;
        int num = 0;
        for (int index = 1; index <= length; ++index)
        {
            num += (int)functionName[length - index] + index;
        }

        return num;
    }

    private static XDocument JsonToXml(string jsonString)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        var quotas = new XmlDictionaryReaderQuotas();
        return XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(stream, quotas));
    }

    public async Task<long> LogRequest(LogAPIRequest request)
    {
        var command = new WriteLogExternalApiInsertCommand(
            request.SystemId,
            request.ProductId,
            request.UserId,
            request.FunctionName,
            request.CustId,
            request.ActionId,
            request.Request.ToString());

        await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

        return command.LogId;
    }

    public async Task LogResponse(long logId, int returnCode, string response)
        => await dynamicRepository.ExecuteAsync(new WriteLogExternalApiUpdResponseCommand(logId, returnCode, response)).ConfigureAwait(false);
}
