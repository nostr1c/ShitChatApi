namespace ShitChat.Application.Caching.Services;

public interface ICacheService
{
    Task StringSetAsync(string key, string value, TimeSpan? expiry = null);
    Task<string?> StringGetAsync(string key);
    Task<bool> SetAddAsync(string key, string value);
    Task<bool> SetRemoveAsync(string key, string value);
    Task<string[]> SetMembersAsync(string key);
    Task KeyDeleteAsync(string key);
}
