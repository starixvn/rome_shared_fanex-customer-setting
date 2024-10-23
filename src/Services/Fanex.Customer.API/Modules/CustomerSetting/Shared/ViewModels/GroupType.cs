namespace Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

public class GroupType
{
    public int Id { get; set; }

    public string Name { get; set; }

    public short DisplayType { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsAllowUncheckAll { get; set; }

    public IEnumerable<GroupItemDefault> GroupItems { get; set; }
}