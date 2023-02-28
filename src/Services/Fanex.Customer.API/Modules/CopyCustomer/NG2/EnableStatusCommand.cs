using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.CopyCustomer.NG2;

public class EnableStatusCommand : NonQueryCommand
{
    public EnableStatusCommand(int custId, int userId, bool disabled, string ip, byte systemId, int subAccId = 0)
    {
        CustId = custId;
        UserId = userId;
        SubAccId = subAccId;
        IP = ip;
        SystemId = systemId;
        IsDisabled = "<ROOT><ProductStatus SportType=\"213\" BetType=\"2501\" IsDisabled=\"" + (disabled ? "1" : "0") + "\" /></ROOT>";
    }

    public int CustId { get; }

    public string IsDisabled { get; }

    public int UserId { get; }

    public int SubAccId { get; }

    public string IP { get; }

    public byte SystemId { get; }

    public override string GetSettingKey() => "Acc_CustSetting_DisableGameStatusUpd";

    public override bool IsValid() => CustId > 0 && UserId > 0;

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }
}