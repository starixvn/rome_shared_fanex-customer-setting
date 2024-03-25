using Fanex.Customer.API.Modules.Message.Models;
using Fanex.Customer.API.Shared.Models;

namespace Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

public class UpdateCustomerResult : Result
{
    public UpdateCustomerResult(int custId, string custName, IEnumerable<UpdateGroupTypeResult> subResults, int code = 0, string message = "")
        : base(code, message)
    {
        CustId = custId;
        CustName = custName;
        SubResults = subResults;
    }

    public int CustId { get; set; }

    public string CustName { get; set; }

    public IEnumerable<UpdateGroupTypeResult> SubResults { get; private set; }
}

public class UpdateGroupTypeResult : CustomerSettingResult
{
    public UpdateGroupTypeResult(Result result)
        :base(result.Code, result.Message)
    {

    }

    public UpdateGroupTypeResult(CustomerSettingResult customerSettingResult)
        :base(customerSettingResult.Code, customerSettingResult.Message, customerSettingResult.SubMessages)
    {

    }
    
    public UpdateGroupTypeResult(
        int productGroupId,
        string productGroupName,
        int groupTypeId,
        string groupTypeName,
         int code = 0,
         string message = "",
          IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message, subMessages)
    {
        ProductGroupId = productGroupId;
        ProductGroupName = productGroupName;
        GroupTypeId = groupTypeId;
        GroupTypeName = groupTypeName;
    }

    public int ProductGroupId { get; set; }

    public string ProductGroupName { get; set; }


    public int GroupTypeId { get; set; }

    public string GroupTypeName { get; set; }
}

public class CreateCustomerResult : CustomerSettingResult
{
    public CreateCustomerResult(int newCustId, int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message, subMessages)
    {
        NewCustId = newCustId;
    }

    public int NewCustId { get; }
}

public class CustomerSettingResult : Result
{
    public CustomerSettingResult(int code = 0, string message = "", IEnumerable<SubMessageContent> subMessages = null)
        : base(code, message)
    {
        SubMessages = subMessages;
    }

    public IEnumerable<SubMessageContent> SubMessages { get; }
}