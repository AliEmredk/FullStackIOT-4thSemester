using api.dtos;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await authService.Login(request.Username, request.Password);
        
        if(token == null)
            return Unauthorized(new { error = "Invalid username or password" });

        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginRequest request)
    {
        await authService.Register(request.Username, request.Password);
        
        return Ok();
    }
}