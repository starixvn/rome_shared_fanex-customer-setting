using Fanex.Data;
using Fanex.Data.Repository;
using Licensee.AdminSetting.API.Features.Shared.SiteDB;

namespace Licensee.AdminSetting.API.Features.Accounts.Criteria
{
    public class CreateDummyMemberCommand(int custDBGroupId, int custId, string username, int creator, int recommend, string ip) : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int CustDBGroupId { get; } = custDBGroupId;

        public int CustId { get; set; } = custId;

        public string Username { get; } = username;

        public int Creator { get; } = creator;

        public int Recommend { get; } = recommend;

        public string IP { get; } = ip;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string ErrExtraInfo { get; set; }

        public override string GetSettingKey() => SiteDBHelpers.GetSPNameInSiteDB(CustDBGroupId, "Acc_Dep_CustomerInsMember_Ag");

        public override bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Username);
    }
}