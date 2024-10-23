using Fanex.Data.Repository;

namespace Licensee.AdminSetting.API.Features.Accounts.Criteria
{
    public class RegisterDummyMemberCommand(int agentId, string site, int siteDBGroupId) : NonQueryCommand
    {
        public int AgentId { get; } = agentId;

        public string Site { get; } = site;

        public int SiteDBGroupId { get; } = siteDBGroupId;

        public override string GetSettingKey() => "API_InsOddsAdj_Register";

        public override bool IsValid() => AgentId > 0;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string Username { get; set; }

        public int CustId { get; set; }
    }
}