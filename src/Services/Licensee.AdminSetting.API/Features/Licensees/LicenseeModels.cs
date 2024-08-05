using System.ComponentModel.DataAnnotations;
using Licensee.AdminSetting.API.Features.Shared;

namespace Licensee.AdminSetting.API.Features.Licensees;

public record BetSetting(
    int ProductId,
    int SportType,
    int BetType,
    int Currency,
    decimal MinBet,
    decimal MaxBet,
    decimal MPM,
    decimal MPB,
    decimal MPPM);

public record PositionTaking(int ProductId, decimal PT);

public record StakeLevel(
    int Currency,
    int GameTypeId,
    decimal MinStake,
    decimal MaxStake);

public record CreateLicenseeRequest(
    [Required] string SiteName,
    [Required] BetSetting[] ProductBetSetting,
    [Required] PositionTaking[] ProductPT,
    [Required] StakeLevel[] ProductStakeLevel,
    [Required] string Creator,
    [Required] string IP,
    [Required] bool IsCreateSub,
    [Required] string LicenseeType,
    [Required] bool IsChinaView,
    [Required] byte SiteDBGroupId,
    [Required] byte SpecialPlatform);

public class CustomerSiteDB
{
    public int GroupId { get; init; }

    public string ServerName { get; init; }
}

public class ValidateCreateLicenseeResult(int errorCode, string errorMessage, string customers)
    : Result(errorCode, errorMessage)
{
    public string Customers { get; } = customers;
}

