using System.ComponentModel.DataAnnotations;
using Licensee.AdminSetting.API.Features.Shared;

namespace Licensee.AdminSetting.API.Features.Licensees;

public record BetSetting(
    int ProductID,
    int Sporttype,
    int Bettype,
    int Currency,
    decimal MinBet,
    decimal MaxBet,
    decimal MPM,
    decimal MPB,
    decimal MPPM);

public record PositionTaking(int ProductID, decimal PT);

public record StakeLevel(
    int Currency,
    int GameTypeID,
    decimal MinStake,
    decimal MaxStake);

public record Commission(int ProductID, decimal Comm);

public record CreateCurrencyRequest(
    [Required] string SiteName,
    [Required] BetSetting[] ProductBetSetting,
    [Required] PositionTaking[] ProductPT,
    [Required] StakeLevel[] ProductStakeLevel,
    [Required] Commission[] ProductComm,
    [Required] string Creator,
    [Required] string IP,
    [Required] bool IsCreateSub,
    [Required] string LicenseeType);

public record CreateLicenseeRequest(
    [Required] string SiteName,
    [Required] BetSetting[] ProductBetSetting,
    [Required] PositionTaking[] ProductPT,
    [Required] StakeLevel[] ProductStakeLevel,
    [Required] Commission[] ProductComm,
    [Required] string Creator,
    [Required] string IP,
    [Required] bool IsCreateSub,
    [Required] string LicenseeType,
    [Required] bool IsChinaView,
    [Required] byte SiteDBGroupId,
    [Required] byte SpecialPlatform)
    : CreateCurrencyRequest(
        SiteName,
        ProductBetSetting,
        ProductPT,
        ProductStakeLevel,
        ProductComm,
        Creator,
        IP,
        IsCreateSub,
        LicenseeType);


public class ValidateCreateLicenseeResult(int errorCode, string errorMessage, string registerUsernames)
    : Result(errorCode, errorMessage)
{
    public string RegisterUsernames { get; } = registerUsernames;
}

public class RegisterCustomersResult(int errorCode, string errorMessage, int siteId, string customers)
    : Result(errorCode, errorMessage)
{
    public int SiteId { get; } = siteId;

    public string Customers { get; } = customers;
}

public class CreateLicenseeResult(int errorCode, string errorMessage, string[] failedCurrencies)
    : Result(errorCode, errorMessage)
{
    public string[] FailedCurrencies { get; } = failedCurrencies;
}


public class RegisterDummyMemberResult(int errorCode, string errorMessage, int custId, string username)
    : Result(errorCode, errorMessage)
{
    public int CustId { get; } = custId;

    public string Username { get; } = username;
}