using HeelmeestersAPI.Features.Shared.Auth.Interfaces;
using HeelmeestersAPI.Features.Shared.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeelmeestersAPI.Features.Shared.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var token = _authService.Authenticate(dto.Username, dto.Password);
        if (token == null) return Unauthorized();

        return Ok(new { Token = token });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-test")]
    public IActionResult AdminTest()
    {
        return Ok("Je bent admin!");
    }

    [Authorize]
    [HttpGet("user-test")]
    public IActionResult UserTest()
    {
        return Ok("Je bent ingelogd!");
    }
}
