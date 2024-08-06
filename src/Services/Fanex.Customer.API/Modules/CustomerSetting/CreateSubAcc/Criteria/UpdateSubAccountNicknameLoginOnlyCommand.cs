using Fanex.Data.Repository;
public class UpdateSubAccountNicknameLoginOnlyCommand : NonQueryCommand
{
    public UpdateSubAccountNicknameLoginOnlyCommand(
        int custId,
        int subAccId,
        string language,
        string ip)
    {
        SubAccId = subAccId;
        CustId = custId;
        IP = ip;
        Language = language;
    }

    public bool NickNameLoginOnly => true;

    public int SubAccId { get; }

    public int CustId { get; }

    public string IP { get; }

    public string Language { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
        => "Age_CustSubUpdNicknameLoginOnly";

    public override bool IsValid() => SubAccId > 0 && CustId > 0;
}