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
    public ActionResult<Token> SSO([FromBody] AuthRequest request)
    {
        var token = _tokenService.GenerateToken(request);

        return Ok(token);
    }
}