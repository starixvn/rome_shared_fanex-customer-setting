namespace Fanex.Customer.API.Modules.Customer.Models;

public class CommissionInfo
{
    public IEnumerable<CommissionHint> CommissionTypeList { get; set; }

    public string MyanmarOddsBetTypesName { get; set; }
}

public class CommissionHint
{
    public int CommissionId { get; set; }

    public string CommissionName { get; set; }

    public IEnumerable<CommissionBettype> Bettypes { get; set; }
}

public class CommissionBettype
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class CommissionInfoDto
{
    public int CommId { get; set; }

    public string TypeId { get; set; }

    public int OrderId { get; set; }
}