using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CreateCustomer.Criteria;

public class UpdateCustomerDateLimitCommand : INonQueryCommand
{
    public UpdateCustomerDateLimitCommand(string custIds, string userNames, DateTime? minDate, int? limitDays, int choice, int userId)
    {
        CustIds = custIds;
        UserNames = userNames;
        MinDate = minDate;
        LimitDays = limitDays;
        Choice = choice;
        UserId = userId;
    }

    [SpParam(SqlParamName = "ListCustID")]
    public string CustIds { get; }

    [SpParam(SqlParamName = "ListUserName")]
    public string UserNames { get; }

    public DateTime? MinDate { get; }

    [SpParam(SqlParamName = "TimePeriod")]
    public int? LimitDays { get; }

    public int Choice { get; }

    public int UserId { get; }

    public int ErrCode { get; }

    public string ErrMsg { get; }

    public string GetSettingKey() => "EAS_CustomerDateLimitMultipleUpd";

    public bool IsValid() => true;
}
