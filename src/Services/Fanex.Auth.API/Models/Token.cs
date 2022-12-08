using System.IdentityModel.Tokens.Jwt;

namespace Fanex.Auth.API.Models;

public record Token(JwtSecurityToken JwtToken, RefreshToken RefreshToken)
{
    public string AccessToken => new JwtSecurityTokenHandler().WriteToken(JwtToken);
}