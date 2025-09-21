using ShitChat.Application.Caching;
using ShitChat.Application.Caching.Services;
using ShitChat.Domain.Entities;
using System.Text.RegularExpressions;

namespace ShitChat.Application.Groups.Services;

public class PresenceService : IPresenceService
{
    private readonly ICacheService _cache;

    public PresenceService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task AddConnectionToGroup(string groupId, string userId, string connectionId)
    {
        var groupUsersKey = CacheKeys.GroupUsers(groupId);
        var userConnectionsKey = CacheKeys.UserConnections(groupId, userId);
        var connectionGroupsKey = CacheKeys.ConnectionGroups(connectionId);

        await _cache.SetAddAsync(groupUsersKey, userId);
        await _cache.SetAddAsync(userConnectionsKey, connectionId);
        await _cache.SetAddAsync(connectionGroupsKey, groupId);
    }

    public async Task RemoveConnection(string userId, string connectionId)
    {
        var connectionGroupsKey = CacheKeys.ConnectionGroups(connectionId);

        var groupIds = await _cache.SetMembersAsync(connectionGroupsKey);
        if (groupIds == null || groupIds.Length == 0)
            return;

        foreach (var groupId in groupIds)
        {
            var userConnectionsKey = CacheKeys.UserConnections(groupId, userId);
            var groupUsersKey = CacheKeys.GroupUsers(groupId);


            await _cache.SetRemoveAsync(userConnectionsKey, connectionId);

            var remainingConnections = await _cache.SetMembersAsync(userConnectionsKey);
                
            if (remainingConnections == null || remainingConnections.Length == 0)
            {
                await _cache.SetRemoveAsync(groupUsersKey, userId);
            }
        }

        foreach (var groupId in groupIds)
        {
            await _cache.SetRemoveAsync(connectionGroupsKey, groupId);
        }
    }

    public async Task<string[]> GetUserConnections(string groupId, string userId)
    {
        var userConnectionsKey = CacheKeys.UserConnections(groupId, userId);

        return await _cache.SetMembersAsync(userConnectionsKey);
    }

    public async Task<string[]> GetUsersInGroup(string groupId)
    {
        var groupUsersKey = CacheKeys.GroupUsers(groupId);

        return await _cache.SetMembersAsync(groupUsersKey);
    }

    public async Task<string[]> GetConnectionGroups(string connectionId)
    {
        var connectionGroupsKey = CacheKeys.ConnectionGroups(connectionId);
        return await _cache.SetMembersAsync(connectionGroupsKey);
    }

}
