using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class RollbackNicknameCommand(int custId, int subAccId) : NonQueryCommand
{
    public int CustId => custId;

    public int SubAccId => subAccId;

    public override string GetSettingKey() => "Age_CustomerInsNickName_Rollback";

    public override bool IsValid() => CustId > 0;
}