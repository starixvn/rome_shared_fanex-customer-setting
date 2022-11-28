using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Fanex.Auth.API.Configurations;
using Fanex.Auth.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fanex.Auth.API.Services;

public interface ITokenService
{
    Token GenerateToken(AuthRequest request);
}


public class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }


    public Token GenerateToken(AuthRequest request)
    {
        var accessToken = GenerateAccessToken(request);
        var requestToken = CreateRefreshToken(request);

        return new Token(accessToken, requestToken);
    }

    private string GenerateAccessToken(AuthRequest request)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", request.UserId.ToString()),
            new Claim(ClaimTypes.Name, request.Username.ToString()),
            new Claim(ClaimTypes.Role, request.RoleId.ToString()),
            new Claim("SubAccId",request.SubAccId.ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CreateRefreshToken(AuthRequest request)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = request.UserId,
            SubAccId = request.SubAccId,
            Expires = DateTime.Now.AddHours(1)
        };

        return refreshToken.Token;
    }
}