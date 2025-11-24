using Microsoft.AspNetCore.Mvc;
using TicketBookingSystem.Api.DTOs;
using TicketBookingSystem.Infrastructure.Services;

namespace TicketBookingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, token, user) = await _authService.RegisterAsync(
            request.Email, request.Password, request.FirstName, request.LastName);

        if (!success || user == null)
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        var response = new AuthResponse(token, user.Id, user.Email, user.FirstName, user.LastName);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, token, user) = await _authService.LoginAsync(request.Email, request.Password);

        if (!success || user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var response = new AuthResponse(token, user.Id, user.Email, user.FirstName, user.LastName);
        return Ok(response);
    }
}
