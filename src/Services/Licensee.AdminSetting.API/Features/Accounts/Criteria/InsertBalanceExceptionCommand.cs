using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Accounts.Criteria
{
    public class InsertBalanceExceptionCommand(int siteDBGroupId, int custId, string username) : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int SiteDBGroupId { get; } = siteDBGroupId;

        public int CustId { get; } = custId;

        public string Username { get; } = username;

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(SiteDBGroupId, "Task_Cust_CheckBalance_Exception_Ins");

        public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Username);
    }
}