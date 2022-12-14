using Fanex.Auth.API.Models;
using Fanex.Auth.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Auth.API;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("sso")]
    public ActionResult<string> SSO([FromBody] AuthRequest request)
    {
        var token = _tokenService.GenerateToken(request.UserId, request.Username, request.RoleId, request.SubAccId);

        return Ok(new { token.AccessToken, RefreshToken = token.RefreshToken.Token });
    }

    [HttpPost("refresh-token")]
    public ActionResult<string> RefreshToken()
    {
        var refreshToken = _tokenService.GetRefreshToken(Request.Cookies["refreshToken"]);

        if (refreshToken == null)
        {
            return Unauthorized("Invalid refresh token.");
        }

        if (refreshToken.Expires < DateTime.Now)
        {
            return Unauthorized("Refresh token expired.");
        }

        var token = _tokenService.GenerateToken(refreshToken.UserId, refreshToken.Username, refreshToken.RoleId, refreshToken.SubAccId);
        SetResponseCookies(token);

        return Ok(token.AccessToken);
    }

    private void SetResponseCookies(Token token)
    {
        Response.Cookies.Append("accessToken", token.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Expires = token.JwtToken.ValidTo
        });
        Response.Cookies.Append("refreshToken", token.RefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Expires = token.RefreshToken.Expires,
        });
    }
}