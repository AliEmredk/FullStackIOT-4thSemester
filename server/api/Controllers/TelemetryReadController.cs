using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/telemetry")]
public class TelemetryReadController(TelemetryCache cache) : ControllerBase
{
    // GET /api/telemetry/turbine-alpha/latest
    [HttpGet("{turbineId}/latest")]
    public IActionResult GetLatest(string turbineId)
    {
        return cache.TryGet(turbineId, out var dto)
            ? Ok(dto)
            : NotFound(new { error = $"No telemetry received yet for '{turbineId}'"});
    }
    
    // GET /api/telemetry/latest (all turbines)
    [HttpGet("latest")]
    public IActionResult LatestAll()
        => Ok(cache.GetAll());
}