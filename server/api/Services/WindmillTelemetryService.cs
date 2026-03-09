using api.dtos;
using dataaccess;
using dataaccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class WindmillTelemetryService : IWindmillTelemetryService
{
    private readonly AppDbContext _db;
    public readonly ILogger<WindmillTelemetryService> _logger;

    public WindmillTelemetryService(AppDbContext db, ILogger<WindmillTelemetryService> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    // Check out what is this cancellationtoken for
    public async Task SaveTelemetryAsync(WindmillTelemetryDto dto, CancellationToken ct = default)
    {
        var turbine = await GetOrCreateTurbineAsync(dto.FarmId, dto.TurbineId, dto.TurbineName, ct);

        var reading = new TelemetryReading
        {
            TurbineIdFk = turbine.Id,
            Timestamp = dto.Timestamp ?? DateTimeOffset.UtcNow,

            WindSpeed = dto.WindSpeed,
            WindDirection = dto.WindDirection,
            AmbientTemperature = dto.AmbientTemperature,
            RotorSpeed = dto.RotorSpeed,
            PowerOutput = dto.PowerOutput,
            NacelleDirection = dto.NacelleDirection,
            BladePitch = dto.BladePitch,
            GeneratorTemp = dto.GeneratorTemp,
            GearboxTemp = dto.GearboxTemp,
            Vibration = dto.Vibration,

            Status = ParseStatus(dto.Status)
        };

        _db.TelemetryReadings.Add(reading);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveAlertAsync(WindmillAlertDto dto, CancellationToken ct = default)
    {
        var turbine = await GetOrCreateTurbineAsync(dto.FarmId, dto.TurbineId, dto.TurbineName, ct);

        if (string.IsNullOrWhiteSpace(dto.Message))
        {
            // no empty alerts storred
            _logger.LogWarning("Skipping alert with empty message for turbineId={TurbineId}", dto.TurbineId);
            return;
        }

        var alert = new AlertEvent
        {
            TurbineIdFk = turbine.Id,
            Timestamp = dto.Timestamp ?? DateTimeOffset.UtcNow,
            Severity = ParseSeverity(dto.Severity),
            Message = dto.Message.Trim()
        };

        _db.AlertEvents.Add(alert);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Turbine> GetOrCreateTurbineAsync(
        string farmId,
        string turbineId,
        string turbineName,
        CancellationToken ct)
    {
        farmId = (farmId ?? "").Trim();
        turbineId = (turbineId ?? "").Trim();
        turbineName = (turbineName ?? "").Trim();

        if (string.IsNullOrWhiteSpace(farmId))
            farmId = "FullStackIOT-ELK"; // fallback if simulator doesn't include it

        if (string.IsNullOrWhiteSpace(turbineId))
            throw new ArgumentException("Telemetry/Alert missing turbineId");

        var turbine = await _db.Turbines
            .FirstOrDefaultAsync(t => t.FarmId == farmId && t.TurbineId == turbineId, ct);

        if (turbine != null)
        {
            //Optional keep name up to date if device send it
            if (!string.IsNullOrWhiteSpace(turbineName) && turbine.TurbineName != turbineName)
                turbine.TurbineName = turbineName;

            return turbine;
        }

        turbine = new Turbine
        {
            FarmId = farmId,
            TurbineId = turbineId,
            TurbineName = string.IsNullOrWhiteSpace(turbineName) ? turbineId : turbineName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Turbines.Add(turbine);

        try
        {
            await _db.SaveChangesAsync(ct);
            return turbine;
        }
        catch (DbUpdateException)
        {
            // Race condition protection: two messages arrive same time, both try to create.
            // Re-query and return the existing one.
            _db.ChangeTracker.Clear();

            var existing = await _db.Turbines
                .FirstAsync(t => t.FarmId == farmId && t.TurbineId == turbineId, ct);

            return existing;
        }
    }

    public async Task<List<WindmillTelemetryDto>> GetHistoryAsync(
        string turbineId,
        DateTimeOffset from,
        DateTimeOffset to,
        int maxPoints = 2000,
        CancellationToken ct = default)
    {
        turbineId = (turbineId ?? "").Trim();
        if (string.IsNullOrWhiteSpace(turbineId))
            throw new ArgumentException("Missing turbineId");

        if (to <= from)
            throw new ArgumentException("Invalid time range");
        
        var turbine = await _db.Turbines
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TurbineId == turbineId, ct);
        
        if (turbine == null)
            return new List<WindmillTelemetryDto>();
        
        var rows = await _db.TelemetryReadings
            .AsNoTracking()
            .Where(r => r.TurbineIdFk == turbine.Id && r.Timestamp >= from && r.Timestamp <= to)
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
            .ToListAsync(ct);
        
        return rows;
    }
    
    public async Task<List<WindmillTelemetryDto>> GetLatestAsync(CancellationToken ct = default)
    {
        var turbines = await _db.Turbines
            .AsNoTracking()
            .ToListAsync(ct);

        var result = new List<WindmillTelemetryDto>();

        foreach (var turbine in turbines)
        {
            var latest = await _db.TelemetryReadings
                .AsNoTracking()
                .Where(r => r.TurbineIdFk == turbine.Id)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync(ct);

            if (latest == null)
                continue;

            result.Add(new WindmillTelemetryDto
            {
                FarmId = turbine.FarmId,
                TurbineId = turbine.TurbineId,
                TurbineName = turbine.TurbineName,
                Timestamp = latest.Timestamp,

                WindSpeed = latest.WindSpeed,
                WindDirection = latest.WindDirection,
                AmbientTemperature = latest.AmbientTemperature,
                RotorSpeed = latest.RotorSpeed,
                PowerOutput = latest.PowerOutput,
                NacelleDirection = latest.NacelleDirection,
                BladePitch = latest.BladePitch,
                GeneratorTemp = latest.GeneratorTemp,
                GearboxTemp = latest.GearboxTemp,
                Vibration = latest.Vibration,

                Status = latest.Status == TurbineStatus.Running ? "running" : "stopped"
            });
        }

        return result;
    }

    private static TurbineStatus ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return TurbineStatus.Running;

        return status.Trim().ToLowerInvariant() switch
        {
            "running" => TurbineStatus.Running,
            "stopped" => TurbineStatus.Stopped,
            "stop" => TurbineStatus.Stopped,
            "start" => TurbineStatus.Running,
            _ => TurbineStatus.Running
        };
    }

    private static AlertSeverity ParseSeverity(string? severity)
    {
        if (string.IsNullOrWhiteSpace(severity))
            return AlertSeverity.Warning;

        return severity.Trim().ToLowerInvariant() switch
        {
            "info" => AlertSeverity.Info,
            "warning" => AlertSeverity.Warning,
            "warn" => AlertSeverity.Warning,
            "critical" => AlertSeverity.Critical,
            "error" => AlertSeverity.Critical,
            _ => AlertSeverity.Warning
        };
    }
}