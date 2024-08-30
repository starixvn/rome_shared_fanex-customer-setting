using System.ComponentModel.DataAnnotations;
using Licensee.AdminSetting.API.Features.Shared;

namespace Licensee.AdminSetting.API.Features.Accounts;

public class CreateDummyMemberResult(int errorCode, string errorMessage, int custId, string username)
    : Result(errorCode, errorMessage)
{
    public int CustId { get; } = custId;

    public string Username { get; } = username;
}

public record CreateDummyMemberRequest([Required] int Creator, string IP);