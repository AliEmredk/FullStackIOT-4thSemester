using api.dtos;
using dataaccess;
using dataaccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.EfRealtime;

namespace api.Controllers;

[ApiController]
[Route("api/realtime")]
public class TelemetryRealtimeController(
    ISseBackplane backplane,
    IRealtimeManager realtimeManager,
    AppDbContext db
) : RealtimeControllerBase(backplane)
{
    [HttpGet("telemetry")]
    public async Task<RealtimeListenResponse<List<WindmillTelemetryDto>>> GetTelemetry(
        string connectionId,
        string turbineId,
        int minutesBack = 60,
        int maxPoints = 1000)
    {
        turbineId = (turbineId ?? "").Trim();

        if (string.IsNullOrWhiteSpace(turbineId))
            throw new ArgumentException("Missing turbineId");

        var group = $"telemetry:{turbineId}";

        await backplane.Groups.AddToGroupAsync(connectionId, group);

        realtimeManager.Subscribe<AppDbContext>(
            connectionId,
            group,
            criteria: changes => changes.HasChanges<TelemetryReading>(),
            query: async ctx =>
            {
                var turbine = await ctx.Turbines
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TurbineId == turbineId);

                if (turbine == null)
                    return new List<WindmillTelemetryDto>();

                var from = DateTimeOffset.UtcNow.AddMinutes(-minutesBack);

                return await ctx.TelemetryReadings
                    .AsNoTracking()
                    .Where(r => r.TurbineIdFk == turbine.Id && r.Timestamp >= from)
                    .OrderByDescending(r => r.Timestamp)
                    .Take(maxPoints)
                    .OrderBy(r => r.Timestamp)
                    .Select(r => new WindmillTelemetryDto
                    {
                        FarmId = turbine.FarmId,
                        TurbineId = turbine.TurbineId,
                        TurbineName = turbine.TurbineName,
                        Timestamp = r.Timestamp,

                        WindSpeed = r.WindSpeed,
                        WindDirection = r.WindDirection,
                        AmbientTemperature = r.AmbientTemperature,
                        RotorSpeed = r.RotorSpeed,
                        PowerOutput = r.PowerOutput,
                        NacelleDirection = r.NacelleDirection,
                        BladePitch = r.BladePitch,
                        GeneratorTemp = r.GeneratorTemp,
                        GearboxTemp = r.GearboxTemp,
                        Vibration = r.Vibration,

                        Status = turbine.CurrentStatus == TurbineStatus.Running ? "running" : "stopped"
                    })
                    .ToListAsync();
            });

        var initialTurbine = await db.Turbines
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TurbineId == turbineId);

        if (initialTurbine == null)
            return new RealtimeListenResponse<List<WindmillTelemetryDto>>(group, new List<WindmillTelemetryDto>());

        var initialFrom = DateTimeOffset.UtcNow.AddMinutes(-minutesBack);

        var initialData = await db.TelemetryReadings
            .AsNoTracking()
            .Where(r => r.TurbineIdFk == initialTurbine.Id && r.Timestamp >= initialFrom)
            .OrderByDescending(r => r.Timestamp)
            .Take(maxPoints)
            .OrderBy(r => r.Timestamp)
            .Select(r => new WindmillTelemetryDto
            {
                FarmId = initialTurbine.FarmId,
                TurbineId = initialTurbine.TurbineId,
                TurbineName = initialTurbine.TurbineName,
                Timestamp = r.Timestamp,

                WindSpeed = r.WindSpeed,
                WindDirection = r.WindDirection,
                AmbientTemperature = r.AmbientTemperature,
                RotorSpeed = r.RotorSpeed,
                PowerOutput = r.PowerOutput,
                NacelleDirection = r.NacelleDirection,
                BladePitch = r.BladePitch,
                GeneratorTemp = r.GeneratorTemp,
                GearboxTemp = r.GearboxTemp,
                Vibration = r.Vibration,

                Status = initialTurbine.CurrentStatus == TurbineStatus.Running ? "running" : "stopped"
            })
            .ToListAsync();

        return new RealtimeListenResponse<List<WindmillTelemetryDto>>(group, initialData);
    }
}