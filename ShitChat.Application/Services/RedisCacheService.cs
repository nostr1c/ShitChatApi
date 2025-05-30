using ShitChat.Application.Interfaces;
using StackExchange.Redis;

namespace ShitChat.Application.Services;

public class RedisCacheService : ICacheService
{
    public readonly IDatabase _db;
    
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }
    public Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        return _db.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public Task RemoveAsync(string key)
    {
        return _db.KeyDeleteAsync(key);
    }
}
