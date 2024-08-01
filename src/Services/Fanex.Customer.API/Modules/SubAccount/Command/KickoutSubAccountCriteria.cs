using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SubAccount.Command;

public class KickoutSubAccountCriteria(int custId, int subAccId) : NonQueryCommand
{
    public int CustId => custId;

    public int SubAccId => subAccId;

    public override string GetSettingKey() => "Age_On_UserKickedout";

    public override bool IsValid() => CustId > 0 && SubAccId > 0;
}