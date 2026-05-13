using StackExchange.Redis;
using System.Text.Json;
using IncidentService.Models;

namespace IncidentService.Services;

public class RedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SaveIncidentAsync(Incident incident)
    {
        var json = JsonSerializer.Serialize(incident);
        await _db.ListRightPushAsync("incidents", json);
    }

    public async Task<List<Incident>> GetIncidentsAsync()
    {
        var values = await _db.ListRangeAsync("incidents");

        var result = new List<Incident>();

        foreach (var v in values)
        {
            var json = v.ToString();

            if (string.IsNullOrEmpty(json))
                continue;

            var obj = JsonSerializer.Deserialize<Incident>(json);

            if (obj != null)
                result.Add(obj);
        }

        return result;
    }
}