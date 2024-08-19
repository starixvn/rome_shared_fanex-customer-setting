using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Accounts.Criteria
{
    public class InsertBalanceExceptionCommand(int custId, string username) : NonQueryCommand
    {
        public int CustId { get; } = custId;

        public string Username { get; } = username;

        public override string GetSettingKey() => "Task_Cust_CheckBalance_Exception_Ins";

        public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Username);
    }
}