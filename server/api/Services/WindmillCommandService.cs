using System.Text.Json;
using dataaccess;
using dataaccess.Entities;
using Mqtt.Controllers;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class WindmillCommandService(
    IMqttClientService mqtt,
    AppDbContext db
    ) : IWindmillCommandService
{
    private static readonly Dictionary<string, (string Name, string Location)> KnownTurbines =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["turbine-alpha"] = ("Alpha", "North Platform"),
            ["turbine-beta"] = ("Beta", "North Platform"),
            ["turbine-gamma"] = ("Gamma", "South Platform"),
            ["turbine-delta"] = ("Delta", "East Platform"),
        };

    private const string FarmId = "FullStackIOT-ELK";

    public async Task SendCommandAsync(string turbineId, JsonElement commandJson, int userId)
    {
        if (!KnownTurbines.ContainsKey(turbineId))
            throw new ArgumentException($"Unknown turbineID '{turbineId}'");
        //checks this later
        if (!commandJson.TryGetProperty("action", out var actionProp) || actionProp.ValueKind != JsonValueKind.String)
            throw new ArgumentException("Command must contain string property 'action'");

        var action = actionProp.GetString()!.Trim();
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Command 'action' must be a non-empty string");
        
        //validate and normalize to a clean JSON payload string
        var (actionEnum, normalizePayload) = ValidateAndNormalize(action, commandJson);
        
        // FK required for turbinecommand
        var turbine = await db.Turbines.SingleOrDefaultAsync(t => t.TurbineId == turbineId);
        if (turbine is null)
        {
            var meta = KnownTurbines[turbineId];
            turbine = new Turbine
            {
                TurbineId = turbineId,
                TurbineName = meta.Name,
                FarmId = FarmId,
            };
            db.Turbines.Add(turbine);
            await db.SaveChangesAsync();
        }

        //Ensure user exists (FK required).
        var userExists = await db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new ArgumentException("Unknown user (token user id not found in the database)");
        
        //Log command
        var cmd = new TurbineCommand
        {
            TurbineIdFk = turbine.Id,
            UserIdFk = userId,
            Timestamp = DateTimeOffset.UtcNow,
            Action = actionEnum,
            PayloadJson = normalizePayload,
            Published = false,
            PublishedAt = null
        };

        db.TurbineCommands.Add(cmd);
        await db.SaveChangesAsync();
        
        // Publish to MQTT
        var topic = $"farm/{FarmId}/windmill/{turbineId}/command";

        try
        {
            await mqtt.PublishAsync(topic, normalizePayload);

            cmd.Published = true;
            cmd.PublishedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync();
        }
        catch
        {
            // Keep cmd.Published=false, command stays logged as "attempted"
            throw;
        }
    }
//check all of these later
    private static (TurbineCommandAction ActionEnum, string NormalizedJson) ValidateAndNormalize(string action,
        JsonElement json)
    {
        //return normalized json for consistent format
        return action switch
        {
            "setInterval" => (TurbineCommandAction.SetInterval, NormalizeSetInterval(json)),
            "stop" => (TurbineCommandAction.Stop, NormalizeStop(json)),
            "start" => (TurbineCommandAction.Start, NormalizeStart()),
            "setPitch" => (TurbineCommandAction.SetPitch, NormalizeSetPitch(json)),
            _ => throw new ArgumentException($"Unsupported action '{action}'")
        };
    }

    private static string NormalizeSetInterval(JsonElement json)
    {
        if (!json.TryGetProperty("value", out var valueProp) || valueProp.ValueKind != JsonValueKind.Number)
            throw new ArgumentException("setInterval requires numeric 'value'");

        var value = valueProp.GetInt32();
        if (value is < 1 or > 60)
            throw new ArgumentException("setInterval 'value' must be between 1 and 60");

        var clean = new { action = "setInterval", value };
        return JsonSerializer.Serialize(clean);
    }

    private static string NormalizeStop(JsonElement json)
    {
        string? reason = null;
        if (json.TryGetProperty("reason", out var reasonProp) && reasonProp.ValueKind == JsonValueKind.String)
            reason = reasonProp.GetString();

        var clean = new { action = "stop", reason };
        return JsonSerializer.Serialize(clean);
    }

    private static string NormalizeStart()
    {
        var clean = new { action = "start" };
        return JsonSerializer.Serialize(clean);
    }

    private static string NormalizeSetPitch(JsonElement json)
    {
        if (!json.TryGetProperty("angle", out var angleProp) || angleProp.ValueKind != JsonValueKind.Number)
            throw new ArgumentException("setPitch requires numeric 'angle'");

        var angle = angleProp.GetDouble();
        if (angle < 0 || angle > 30)
            throw new ArgumentException("setPitch 'angle' must be between 0 and 30");

        var clean = new { action = "setPitch", angle };
        return JsonSerializer.Serialize(clean);
    }

}