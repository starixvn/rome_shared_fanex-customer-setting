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
    Token GenerateToken(int userId, string username, byte roleId, int subAccId);

    RefreshToken? GetRefreshToken(string? refreshToken);
}

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private RefreshToken? _refreshToken = null;

    public TokenService(IOptions<JwtOptions> options)
        => _options = options.Value;

    public Token GenerateToken(int userId, string username, byte roleId, int subAccId)
    {
        var accessToken = GenerateAccessToken(userId, username, roleId, subAccId);
        var refreshToken = CreateRefreshToken(userId, username, roleId, subAccId);

        return new Token(accessToken, refreshToken);
    }

    public RefreshToken? GetRefreshToken(string? refreshToken)
    {
        if (_refreshToken != null && _refreshToken.Token.Equals(refreshToken))
        {
            return _refreshToken;
        }

        return null;
    }

    private JwtSecurityToken GenerateAccessToken(int userId, string username, byte roleId, int subAccId)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", userId.ToString()),
            new Claim(ClaimTypes.Name, username.ToString()),
            new Claim(ClaimTypes.Role, roleId.ToString()),
            new Claim("SubAccId",subAccId.ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        return new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);
    }

    private RefreshToken CreateRefreshToken(int userId, string username, byte roleId, int subAccId)
    {
        _refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            Username = username,
            RoleId = roleId,
            SubAccId = subAccId,
            Expires = DateTime.Now.AddHours(2)
        };

        return _refreshToken;
    }
}