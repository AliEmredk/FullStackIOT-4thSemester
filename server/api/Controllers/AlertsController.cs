using dataaccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAlerts(string turbineId)
    {
        var turbine = await db.Turbines
            .FirstOrDefaultAsync(t => t.TurbineId == turbineId);

        if (turbine == null)
            return Ok(new List<object>());

        var alerts = await db.AlertEvents
            .Where(a => a.TurbineIdFk == turbine.Id)
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .Select(a => new
            {
                message = a.Message,
                severity = a.Severity.ToString(),
                timestamp = a.Timestamp
            })
            .ToListAsync();

        return Ok(alerts);
    }
}