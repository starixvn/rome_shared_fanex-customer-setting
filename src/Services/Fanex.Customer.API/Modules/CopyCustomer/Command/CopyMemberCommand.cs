using Fanex.Data;

namespace Fanex.Customer.API.Modules.CopyCustomer.Command;

public class CopyMemberCommand : CopyCustomerCommand
{
    public CopyMemberCommand(
        string userName,
        string userpwd,
        decimal credit,
        bool isQrCode,
        int creator,
        int recommend,
        int sourceCustId,
        int subAccountId,
        string language,
        string ip,
        byte systemId,
        byte settingStatus)
        : base(userName, userpwd, credit, creator, sourceCustId, language, ip, systemId, settingStatus)
    {
        IsQrCode = isQrCode;
        Recommend = recommend;
        SubAccId = subAccountId;
    }

    public int Recommend { get; }

    public bool IsSearchAndCopy { get; } = true;

    public bool IsQrCode { get; }

    public int SubAccId { get; }

    public int MsgType { get; set; }

    public override string GetSettingKey() => "Acc_CustomerInsMember_Ag";
}
