using api.dtos;
using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.EfRealtime;

namespace api.Services;

public class TelemetryRealtimeService : ITelemetryRealtimeService
{
    private readonly ISseBackplane _backplane;
    private readonly IRealtimeManager _realtimeManager;
    private readonly AppDbContext _db;

    public TelemetryRealtimeService(
        ISseBackplane backplane,
        IRealtimeManager realtimeManager,
        AppDbContext db)
    {
        _backplane = backplane;
        _realtimeManager = realtimeManager;
        _db = db;
    }

    public async Task<RealtimeListenResponse<List<WindmillTelemetryDto>>> GetTelemetryAsync(
        string connectionId,
        string turbineId,
        int minutesBack = 60,
        int maxPoints = 1000)
    {
        turbineId = (turbineId ?? "").Trim();

        if (string.IsNullOrWhiteSpace(turbineId))
            throw new ArgumentException("Missing turbineId");

        var group = $"telemetry:{turbineId}";

        await _backplane.Groups.AddToGroupAsync(connectionId, group);

        _realtimeManager.Subscribe<AppDbContext>(
            connectionId,
            group,
            criteria: changes => changes.HasChanges<TelemetryReading>(),
            query: async ctx => await LoadTelemetryAsync(ctx, turbineId, minutesBack, maxPoints));

        var initialData = await LoadTelemetryAsync(_db, turbineId, minutesBack, maxPoints);

        return new RealtimeListenResponse<List<WindmillTelemetryDto>>(group, initialData);
    }

    private async Task<List<WindmillTelemetryDto>> LoadTelemetryAsync(
        AppDbContext ctx,
        string turbineId,
        int minutesBack,
        int maxPoints)
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

                Status = r.Status == TurbineStatus.Running ? "running" : "stopped"
            })
            .ToListAsync();
    }
}