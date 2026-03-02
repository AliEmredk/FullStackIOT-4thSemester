using System.Security.Claims;
using System.Text.Json;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/windmills")]
public class WindmillCommandsController(IWindmillCommandService svc) : ControllerBase
{
    // /api/windmills/{turbineId}/command
    [Authorize]
    [HttpPost("{turbineId}/command")]
    public async Task<IActionResult> SendCommand(string turbineId, [FromBody] JsonElement command)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { error = "Missing/invalid NameIdentifier claim" });

        try
        {
            await svc.SendCommandAsync(turbineId, command, userId);
            return Ok(new { ok = true });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    
}