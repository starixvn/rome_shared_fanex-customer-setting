using Fanex.Customer.API.Shared.Utils;
using Fanex.Data.Repository;
public class UpdateSubAccountNicknameLoginOnlyCommand : NonQueryCommand
{
    public UpdateSubAccountNicknameLoginOnlyCommand(
        int custId,
        int subAccId,
        string language,
        string ip,
        int custDBGroupID)
    {
        SubAccId = subAccId;
        CustId = custId;
        IP = ip;
        Language = language;
        CustDBGroupID = custDBGroupID;
    }

    public bool NickNameLoginOnly => true;

    public int SubAccId { get; }

    public int CustId { get; }

    public string IP { get; }

    public int CustDBGroupID { get; }

    public string Language { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
        => SPNameUtils.GetSPNameWithPrefix("Age_CustSubUpdNicknameLoginOnly", CustDBGroupID);

    public override bool IsValid() => SubAccId > 0 && CustId > 0;
}