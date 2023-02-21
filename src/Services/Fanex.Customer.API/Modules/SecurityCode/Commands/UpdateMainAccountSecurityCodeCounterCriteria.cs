using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.SecurityCode.Commands;

public class UpdateMainAccountSecurityCodeCounterCommand : UpdateSecurityCodeCounterCommandBase, INonQueryCommand
{
    public UpdateMainAccountSecurityCodeCounterCommand(int custId, bool checkedSecurityCode, string language, string ip)
        : base(custId, checkedSecurityCode, language, ip)
    {
    }

    public string GetSettingKey() => "Age_UpdateMainAccountSecurityCodeCounter";

    public bool IsValid() => CustId > 0;
}