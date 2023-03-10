using Fanex.Data.Repository;

namespace Fanex.Customer.API.Modules.Customer.Criteria;

public class CheckExistedCustomerCommand : NonQueryCommand
{
    public CheckExistedCustomerCommand(string username)
    {
        Username = username;
    }

    public string Username { get; }

    public bool IsExists { get; set; }

    public override string GetSettingKey() => "Age_CustomerCheckExists";
    public override bool IsValid() => !string.IsNullOrEmpty(Username);
}