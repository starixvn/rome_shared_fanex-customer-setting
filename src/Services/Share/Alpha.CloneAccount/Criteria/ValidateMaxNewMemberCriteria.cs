using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Criteria;

public class ValidateMaxNewMemberCriteria : NonQueryCommand
{
    public ValidateMaxNewMemberCriteria(int agentId, string language)
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
