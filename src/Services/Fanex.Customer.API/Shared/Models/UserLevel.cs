using Fanex.Customer.API.Shared.Enumerations;

namespace Fanex.Customer.API.Shared.Models;

[Serializable]
public class UserLevel : Enumeration
{

    public static readonly UserLevel Company = new(5, "Company");

    public static readonly UserLevel Super = new(4, "Super");

    public static readonly UserLevel Master = new(3, "Master");

    public static readonly UserLevel Agent = new(2, "Agent");

    public static readonly UserLevel Member = new(1, "Member");

    public UserLevel()
    {
    }

    private UserLevel(byte value, string displayName)
        : base(value, displayName)
    {
    }

    public bool IsAgentLevel() => this == Agent;

    public bool IsMasterLevel() => this == Master;

    public bool IsSuperLevel() => this == Super;

    public bool IsCompanyLevel() => this == Company;

    public UserLevel GetDirectDownLineLevel() => FromValue<UserLevel>((byte)(Value - 1));

    public UserLevel GetDirectUpLineLevel() => FromValue<UserLevel>((byte)(Value + 1));
}