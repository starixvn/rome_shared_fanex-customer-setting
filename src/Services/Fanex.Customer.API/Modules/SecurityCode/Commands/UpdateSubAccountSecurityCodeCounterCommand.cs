using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SecurityCode.Commands;

public class UpdateSubAccountSecurityCodeCounterCommand : UpdateSecurityCodeCounterCommandBase, INonQueryCommand
{
    public UpdateSubAccountSecurityCodeCounterCommand(int custId, int subAccId, bool checkedSecurityCode, string language, string ip)
        : base(custId, checkedSecurityCode, language, ip)
    {
        SubAccId = subAccId;
    }

    public int SubAccId { get; }

    public string GetSettingKey() => "Age_UpdateSubAccountSecurityCodeCounter";

    public bool IsValid() => CustId > 0;
}