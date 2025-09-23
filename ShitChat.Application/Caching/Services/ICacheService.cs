using StackExchange.Redis;

namespace ShitChat.Application.Caching.Services;

public interface ICacheService
{
    Task StringSetAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> StringGetAsync(string key);
    Task<bool> SetAddAsync(string key, string value);
    Task<bool> SetRemoveAsync(string key, string value);
    Task<string[]> SetMembersAsync(string key);
    Task KeyDeleteAsync(string key);
    Task<long> ListLeftPushAsync(string key, string[] value);
    Task<long> ListRightPushAsync(string key, string[] value);
    Task<long> ListLeftPushAsync(string key, string value);
    Task<long> ListRightPushAsync(string key, string value);
    Task ListTrimAsync(string key, long start, long stop);
    Task<RedisValue[]> ListRangeAsync(string key, long start = 0, long stop = -1);
    Task<bool> KeyExpireAsync(string key, TimeSpan? expiry);
}
