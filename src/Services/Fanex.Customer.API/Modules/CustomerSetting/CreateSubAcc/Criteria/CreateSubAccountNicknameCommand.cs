using Fanex.Customer.API.Shared.Utils;
using Fanex.Data;
using Fanex.Data.Repository;

public class CreateSubAccountNicknameCommand: NonQueryCommand
{
    public CreateSubAccountNicknameCommand(
        int custId, int subAccId, string nickname, string language, string ip, int custDBGroupID)
    {
        NickName = nickname;
        SubAccId = subAccId;
        CustId = custId;
        IP = ip;
        Language = language;
        CustDBGroupID = custDBGroupID;
    }

    public string NickName { get; }

    public int SubAccId { get; }

    public int CustId { get; }

    public string IP { get; }

    [SpParam(Ignored = true)]
    public int CustDBGroupID { get; }

    public string Language { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
        => SPNameUtils.GetSPNameWithPrefix("Age_CustSubInsNickname", CustDBGroupID);

    public override bool IsValid() => SubAccId > 0 && CustId > 0;
}