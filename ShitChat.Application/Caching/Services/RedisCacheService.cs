using StackExchange.Redis;

namespace ShitChat.Application.Caching.Services;

public class RedisCacheService : ICacheService
{
    public readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    // String
    public Task StringSetAsync(string key, string value, TimeSpan? expiry = null)
    {
        return _db.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> StringGetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    // Set
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

    // List
    public Task<long> ListRightPushAsync(string key, string[] values)
    {
        var redisValues = values.Select(v => (RedisValue)v).ToArray();
        return _db.ListRightPushAsync(key, redisValues);
    }

    public Task<long> ListLeftPushAsync(string key, string[] values)
    {
        var redisValues = values.Select(v => (RedisValue)v).ToArray();
        return _db.ListLeftPushAsync(key, redisValues);
    }

    public Task<long> ListRightPushAsync(string key, string value)
    {
        return _db.ListRightPushAsync(key, value);
    }

    public Task<long> ListLeftPushAsync(string key, string value)
    {
        return _db.ListLeftPushAsync(key, value);
    }

    public Task ListTrimAsync(string key, long start, long stop)
    {
        return _db.ListTrimAsync(key, start, stop);
    }

    public async Task<RedisValue[]> ListRangeAsync(string key, long start = 0, long stop = -1)
    {
        return await _db.ListRangeAsync(key, start, stop);
    }

    // Key
    public Task KeyDeleteAsync(string key)
    {
        return _db.KeyDeleteAsync(key);
    }

    public Task<bool> KeyExpireAsync(string key, TimeSpan? expiry)
    {
        return _db.KeyExpireAsync(key, expiry);
    }
}
