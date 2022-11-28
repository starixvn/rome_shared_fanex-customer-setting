namespace Fanex.Auth.API.Models;

public class RefreshToken
{

    public string Token { get; init; } = string.Empty;

    public int UserId { get; init; }

    public string Username { get; init; } = string.Empty;

    public byte RoleId { get; init; }

    public int SubAccId { get; init; }

    public bool IsUsed { get; set; } = false;

    public bool IsRevoked { get; set; } = false;

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime Expires { get; init; }
}