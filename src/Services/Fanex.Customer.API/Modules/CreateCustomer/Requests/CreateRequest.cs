using Fanex.Customer.API.Modules.CreateCustomer.ViewModels;

namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class CreateContext
{
    public int CreatorId { get; set; }

    public int SubAccId { get; set; }

    public string IP { get; set; }

    public short SystemId { get; set; }

    public string Language { get; set; }

    public UIBehavior UIBehavior { get; set; }
}

public class UIBehavior
{
    public IEnumerable<UIProductGroup> ProductGroups { get; set; }
}

public class UIProductGroup
{
    public int Id { get; set; }

    public IEnumerable<UIGroupType> GroupTypes { get; set; }
}

public class UIGroupType
{
    public int Id { get; set; }

    public IEnumerable<UISubGroup> SubGroups { get; set; }
}

public class UISubGroup
{
    public int Id { get; set; }

    public IEnumerable<UIAttribute> Attributes { get; set; }

}

public class UIAttribute
{
    public string Key { get; set; }

    public string Value { get; set; }
}

public class CreateRequest
{
    public CreateContext Context { get; set; }

    public CustomerSetting Settings { get; set; }
}
