using Fanex.Data;
using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class GetSuggestCashMemberUsernameCommand : NonQueryCommand
{
    public GetSuggestCashMemberUsernameCommand(string uplineUsername)
    {
        UplineUsername = uplineUsername;
    }

    public string UplineUsername { get; }

    public string SeqName { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelSuggestUsername_CashMember";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(UplineUsername);
}