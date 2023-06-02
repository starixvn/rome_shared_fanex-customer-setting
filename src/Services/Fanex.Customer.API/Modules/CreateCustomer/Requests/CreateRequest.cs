using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class CreateContext
{
    public int CreatorId { get; set; }

    public int SubAccId { get; set; }

    public string IP { get; set; }

    public short SystemId { get; set; }

    public string Language { get; set; }
}

public class CreateRequest
{
    public CreateContext Context { get; set; }

    public CustomerSetting Settings { get; set; }
}
