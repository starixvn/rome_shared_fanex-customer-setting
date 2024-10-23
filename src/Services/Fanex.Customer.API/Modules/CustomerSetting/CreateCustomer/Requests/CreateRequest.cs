using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Modules.CreateCustomer.Requests;

public class RequestContext
{
    public int CreatorId { get; set; }

    public int SubAccId { get; set; }

    public string Site { get; set; }

    public string SiteLabel { get; set; }

    public string IP { get; set; }

    public byte SystemId { get; set; }

    public int CustDBGroupID { get; set; }

    public string Language { get; set; }

    /// <summary>
    /// 0: Lite Mode
    /// 1: Full Mode
    /// </summary>
    public int Mode { get; set; }

    public UIBehavior UIBehavior { get; set; }

    public bool IsFullMode() => Mode == 1;
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
    public RequestContext Context { get; set; }

    public CustomerSettingModel Settings { get; set; }
}
