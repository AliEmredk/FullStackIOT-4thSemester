using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly IWindmillTelemetryService _svc;

    public TelemetryController(IWindmillTelemetryService svc)
    {
        _svc = svc;
    }

    [HttpGet("latest")]
    public async Task<IActionResult> Latest(CancellationToken ct = default)
    {
        var rows = await _svc.GetLatestAsync(ct);
        return Ok(rows);
    }

    [HttpGet("history")]
    public async Task<IActionResult> History(
        [FromQuery] string turbineId,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        [FromQuery] int maxPoints = 2000,
        CancellationToken ct = default)
    {
        var rows = await _svc.GetHistoryAsync(turbineId, from, to, maxPoints, ct);
        return Ok(rows);
    }
}