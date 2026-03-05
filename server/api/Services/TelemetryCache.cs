using System.Collections.Concurrent;
using api.dtos;

namespace api.Services;

public class TelemetryCache
{
    private readonly ConcurrentDictionary<string, WindmillTelemetryDto> _latest = new();
    
    public void Upsert(WindmillTelemetryDto dto) 
        => _latest[dto.TurbineId] = dto;
    
    public bool TryGet(string turbineId, out WindmillTelemetryDto dto)
        => _latest.TryGetValue(turbineId, out dto!);
    
    public IReadOnlyCollection<WindmillTelemetryDto> GetAll()
        => _latest.Values.ToList();
}