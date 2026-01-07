using Microsoft.AspNetCore.Mvc;
using Expenses.Api.Services;

namespace Expenses.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] AuthRequest request)
    {
        var result = await _authenticationService.RegisterAsync(request.Email, request.Password);

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse(result.ErrorMessage ?? "Registration failed"));
        }

        return Ok(new AuthResponse(result.Token!, result.UserId!.Value));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var result = await _authenticationService.AuthenticateAsync(request.Email, request.Password);

        if (!result.Success)
        {
            return Unauthorized(new ErrorResponse(result.ErrorMessage ?? "Authentication failed"));
        }

        return Ok(new AuthResponse(result.Token!, result.UserId!.Value));
    }
}

public record AuthRequest(string Email, string Password);
public record AuthResponse(string Token, Guid UserId);
public record ErrorResponse(string Message);
