using StackExchange.Redis;
using System.Text.Json;
using IncidentService.Models;

namespace IncidentService.Services;

public class RedisCacheService
{
    private readonly IDatabase _db;
    // FIX: Store the endpoint string to use in GetServer() instead of hardcoding
    private readonly string _redisEndpoint;

    public RedisCacheService(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _db = redis.GetDatabase();

        // FIX: Parse the host:port from config so GetServer() works in any environment
        var connStr = configuration["Redis:Connection"] ?? "redis:6379";
        // Connection string may include options like "redis:6379,abortConnect=false"
        // Take only the first host:port token
        _redisEndpoint = connStr.Split(',')[0].Trim();
    }

    public async Task SaveIncidentAsync(Incident incident)
    {
        string key = $"incident:{incident.ServiceName}";

        bool exists = await _db.KeyExistsAsync(key);

        if (exists)
        {
            return;
        }

        var json = JsonSerializer.Serialize(incident);

        await _db.StringSetAsync(
            key,
            json,
            TimeSpan.FromHours(24));
    }

    public async Task<List<Incident>> GetIncidentsAsync()
    {
        // FIX: Use the config-driven endpoint instead of hardcoded "redis:6379"
        var server = _db.Multiplexer.GetServer(_redisEndpoint);

        var keys = server.Keys(pattern: "incident:*");

        var incidents = new List<Incident>();

        foreach (var key in keys)
        {
            var json = await _db.StringGetAsync(key);

            if (json.IsNullOrEmpty)
                continue;

            var incident =
                JsonSerializer.Deserialize<Incident>(
                    json!,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (incident != null)
            {
                incidents.Add(incident);
            }
        }

        return incidents;
    }

    public async Task RemoveIncidentAsync(string serviceName)
    {
        await _db.KeyDeleteAsync(
            $"incident:{serviceName}");
    }
}