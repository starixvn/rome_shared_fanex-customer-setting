using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.EditCustomer.Requests;

public class EditMultipleRequestContext : EditRequestContext
{
    public string CustomerIds { get; set; }

    public string CustomerNames { get; set; }

    internal List<EditMultipleCustomerItem> UpdatedCustomerList { get; set; }
}

public class EditMultipleCustomerItem
{
    public int CustId { get; set; }

    public string UserName { get; set; }
}

public class EditRequestContext
{
    public int ActorId { get; set; }

    public int SubAccId { get; set; }

    public int CustId { get; set; }

    public string UserName { get; set; }

    public byte RoleId { get; set; }

    public byte UplineRoleId { get; set; }

    public string IP { get; set; }

    public string Site { get; set; }

    public string SiteLabel { get; set; }

    public byte SystemId { get; set; }

    public string Language { get; set; }
}

public class EditRequest
{
    public EditRequestContext Context { get; set; }

    public CustomerSetting Settings { get; set; }
}

public class EditMultipleRequest: EditRequest
{
    public new EditMultipleRequestContext Context { get; set; }
}
