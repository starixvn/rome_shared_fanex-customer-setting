using System.Collections;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Customer.API.Shared.Services;
using Fanex.Customer.API.Modules.Message;
using Fanex.Customer.API.Modules.CreateSubAcc.Requests;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Shared.Utils;
using Fanex.Customer.API.Modules.CopyCustomer.Command;

namespace Fanex.Customer.API.Modules.CreateSubAcc.Services;
public interface ICreateSubAccountService
{
    bool ValidateRequest(CreateSubAccRequest request);
    Task<CustomerSettingResult> Create(CreateSubAccRequest request);
}

public class CreateSubAccService : BaseSubAccountService, ICreateSubAccountService
{
    private readonly Logging.ILogger logger;
    private readonly IDynamicRepository dynamicRepository;

    public CreateSubAccService(IMessageService messageService, IScaleoutDBService scaleoutDBService, Logging.ILogger logger, IDynamicRepository dynamicRepository) : base(messageService, scaleoutDBService)
    {
        this.logger = logger;
        this.dynamicRepository = dynamicRepository;
    }
    private const string RegisterSPName = "Age_CustSubIns_Register";

    private const string CreateNewSPName = "Age_CustSubIns";

    private const string RollbackSPName = "Acc_CustomerIns_Rollback";

    public bool ValidateRequest(CreateSubAccRequest request) => true;

    protected override async Task<CreateCustomerResult> Register(CreateSubAccRequest request)
    {
        var parameters = new Hashtable
        {
            { "username", request.UserName },
            { "sitedbgroupid", await scaleoutDBService.GetCustDBGroupIdBySite(request.Site) },
            { "language", LanguageUtils.ToLanguageKey(request.Language) },
            { "custid", request.CustId },
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RegisterSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var message = messageService.TranslateMessage(Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]), request.Language);

        return new CreateCustomerResult(Convert.ToInt32(outputParams["subaccid"]), Convert.ToInt32(outputParams["errcode"]), message.MainMessage, message.SubMessages);
    }

    protected override async Task<CreateCustomerResult> DoProcessing(CreateSubAccRequest request, int subAccId)
    {
        var parameters = new Hashtable
        {
            { "subaccid", subAccId },
            { "custid", request.CustId },
            { "username", request.UserName },
            { "userpwd", request.Password},
            { "firstname", request.FirstName },
            { "lastname", request.LastName },
            { "pageaccess", request.PageAccess },
            { "loginsubaccid", request.LoginSubAccId},
            { "language", LanguageUtils.ToLanguageKey(request.Language) },
            { "userid", request.UserId },
            { "loginip", request.IP }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(CreateNewSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();
        var msg = messageService.TranslateMessage(Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]), request.Language);

        Task.Run(() => { InsertUnreadMessage(request.CustId, request.RoleId, subAccId); });

        return new CreateCustomerResult(subAccId, Convert.ToInt32(outputParams["errcode"]), msg.MainMessage, msg.SubMessages);
    }

    protected override async void Rollback(int custId, int subAccId)
    {
        var parameters = new Hashtable
        {
            { "custid", custId },
            { "SubAccId", subAccId }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(RollbackSPName);
        await db.ExecuteNonQueryAsync(parameters);
    }

    private async Task InsertUnreadMessage(int custId, int roleId, int subAccId)
    {
        var criteria = new CustomerReadMessageCommand(custId, roleId, subAccId, 0, string.Empty);
        await dynamicRepository.ExecuteAsync(criteria);

        if (criteria.ErrCode != 0)
        {
            await logger.InfoAsync($"[Age_MsgCustReadStatus_Ins] CustId: {custId}; SubAccId: 0; RoleId: {roleId} ErrCode: {criteria.ErrCode}; ErrMsg: {criteria.ErrMsg}");
        }
    }
}