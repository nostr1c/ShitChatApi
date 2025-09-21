using StackExchange.Redis;

namespace ShitChat.Application.Caching.Services;

public class RedisCacheService : ICacheService
{
    public readonly IDatabase _db;
    
    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }
    public Task StringSetAsync(string key, string value, TimeSpan? expiry = null)
    {
        return _db.StringSetAsync(key, value, expiry);
    }

    public Task<bool> SetAddAsync(string key, string value)
    {
        return _db.SetAddAsync(key, value);
    }
    public Task<bool> SetRemoveAsync(string key, string value)
    {
        return _db.SetRemoveAsync(key, value);
    }
    public async Task<string[]> SetMembersAsync(string key)
    {
        var members = await _db.SetMembersAsync(key);
        return members.Select(v => v.ToString()).ToArray();
    }


    public async Task<string?> StringGetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public Task KeyDeleteAsync(string key)
    {
        return _db.KeyDeleteAsync(key);
    }
}
