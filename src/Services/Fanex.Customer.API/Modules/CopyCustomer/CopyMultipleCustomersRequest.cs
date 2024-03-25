namespace Fanex.Customer.API.Modules.CopyCustomer;

public class CopyMultipleCustomersRequest : CopyCustomerRequest
{
    public int NumberOfAccounts { get; set; }

    public override bool IsValid()
        => base.IsValid() && NumberOfAccounts > 0;
}