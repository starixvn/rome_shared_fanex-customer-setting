using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetSuggestDirectMemberUsernameCommand : NonQueryCommand
{
    public GetSuggestDirectMemberUsernameCommand(string uplineUsername)
    {
        UplineUsername = uplineUsername;
    }

    public string UplineUsername { get; }

    public string SeqName { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelSuggestUsername_DirectMember";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(UplineUsername);
}