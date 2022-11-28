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
        SetResponseRefreshToken(token.RefreshToken);

        return Ok(token.AccessToken);
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
        SetResponseRefreshToken(token.RefreshToken);

        return Ok(token.AccessToken);
    }

    private void SetResponseRefreshToken(RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }
}