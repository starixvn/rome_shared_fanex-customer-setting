using Fanex.Data.Repository;

public class CreateSubAccountNicknameCommand: NonQueryCommand
{
    public CreateSubAccountNicknameCommand(
        int custId, int subAccId, string nickname, string language, string ip)
    {
        NickName = nickname;
        SubAccId = subAccId;
        CustId = custId;
        IP = ip;
        Language = language;
    }

    public string NickName { get; }

    public int SubAccId { get; }

    public int CustId { get; }

    public string IP { get; }

    public string Language { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey()
        => "Age_CustSubInsNickname";

    public override bool IsValid() => SubAccId > 0 && CustId > 0;
}