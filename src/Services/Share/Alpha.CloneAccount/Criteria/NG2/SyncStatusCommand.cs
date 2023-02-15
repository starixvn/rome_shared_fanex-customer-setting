using Fanex.Data.Repository;

namespace Alpha.CloneAccount.Criteria.NG2;

public class SyncStatusCommand : NonQueryCommand
{
    public SyncStatusCommand(int custId, int userId, string ip, int subAccId = 0)
    {
        CustId = custId;
        UserId = userId;
        SubAccId = subAccId;
        IP = ip;
    }

    public int CustId { get; }

    public bool IsSynced { get; } = true;

    public int ProductId { get; } = Constants.ProductId.NG2;

    public int UserId { get; }

    public int SubAccId { get; }

    public string IP { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Acc_CustSetting_SyncGameStatusUpd";

    public override bool IsValid() => CustId > 0 && UserId > 0;
}