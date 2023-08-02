using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command
{
    public class CheckMaxNewMemberBySuperCommand : NonQueryCommand
    {
        public CheckMaxNewMemberBySuperCommand(int agentId, string language)
        {
            AgentId = agentId;
            Language = language;
        }

        public int AgentId { get; }

        public string Language { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "Age_MaxMemberLimitBySuperCheck";

        public override bool IsValid()
            => AgentId > 0;
    }
}
