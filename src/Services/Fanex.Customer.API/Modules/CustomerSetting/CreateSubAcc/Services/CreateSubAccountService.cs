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
    bool ValidateRequest(BaseSubAccountRequest request);
    Task<CustomerSettingResult> Create(BaseSubAccountRequest request);
}

public class CreateSubAccService : BaseSubAccountService, ICreateSubAccountService
{
    private readonly Logging.ILogger logger;

    public CreateSubAccService(IMessageService messageService, IScaleoutDBService scaleoutDBService, Logging.ILogger logger, IDynamicRepository dynamicRepository) 
        : base(messageService, scaleoutDBService, dynamicRepository)
    {
        this.logger = logger;
    }
    
    private const string CreateNewSPName = "Age_CustSubIns";

    protected override int IsResetNickName => 0;

    public bool ValidateRequest(BaseSubAccountRequest request) => true;

    protected override async Task<CreateCustomerResult> DoProcessing(BaseSubAccountRequest request, int subAccId)
    {
        var createRequest = (CreateSubAccountRequest)request;
        var parameters = new Hashtable
        {
            { "subaccid", subAccId },
            { "custid", createRequest.CustId },
            { "username", createRequest.UserName },
            { "userpwd", createRequest.Password},
            { "firstname", createRequest.FirstName },
            { "lastname", createRequest.LastName },
            { "pageaccess", createRequest.PageAccess },
            { "loginsubaccid", createRequest.LoginSubAccId},
            { "language", LanguageUtils.ToLanguageKey(createRequest.Language) },
            { "userid", createRequest.UserId },
            { "loginip", createRequest.IP }
        };

        using IObjectDb db = ObjectDbFactory.CreateInstance(CreateNewSPName);
        await db.ExecuteNonQueryAsync(parameters);
        var outputParams = db.GetOutputParams();        

        Task.Run(() => { InsertUnreadMessage(createRequest.CustId, createRequest.RoleId, subAccId); });

        return new CreateCustomerResult(subAccId, Convert.ToInt32(outputParams["errcode"]), Convert.ToString(outputParams["errmsg"]));
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