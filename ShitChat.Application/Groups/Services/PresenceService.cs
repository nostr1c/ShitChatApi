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
        var userConnectionsKey = CacheKeys.UserConnections(userId);
        var groupUsersKey = CacheKeys.GroupUsers(groupId);
        var userGroupsKey = CacheKeys.UserGroups(userId);

        // Add connection
        await _cache.SetAddAsync(userConnectionsKey, connectionId);

        // Add group to users group list
        await _cache.SetAddAsync(userGroupsKey, groupId);

        // Add user to group (ONLY IF FIRST CONNECTION)
        var userConnections = await _cache.SetMembersAsync(userConnectionsKey);

        if (userConnections.Length == 1)
        {
            await _cache.SetAddAsync(groupUsersKey, userId);
        }
    }

    public async Task RemoveConnection(string userId, string connectionId)
    {
        var userConnectionsKey = CacheKeys.UserConnections(userId);
        var userGroupsKey = CacheKeys.UserGroups(userId);

        // Remove connection
        await _cache.SetRemoveAsync(userConnectionsKey, connectionId);

        // Has connections still = keep online
        var remainingConnections = await _cache.SetMembersAsync(userConnectionsKey);
        if (remainingConnections != null && remainingConnections.Length > 0)
            return;

        // No more connections = remove user from all groups
        var groups = await _cache.SetMembersAsync(userGroupsKey);
        if (groups != null)
        {
            foreach (var groupId in groups)
            {
                var groupUsersKey = CacheKeys.GroupUsers(groupId);
                await _cache.SetRemoveAsync(groupUsersKey, userId);
                await _cache.SetRemoveAsync(userGroupsKey, groupId);
            }
        }
    }
    public async Task<string[]> GetUsersInGroup(string groupId)
    {
        var groupUsersKey = CacheKeys.GroupUsers(groupId);
        return await _cache.SetMembersAsync(groupUsersKey);
    }

    public async Task<string[]> GetUserConnections(string userId)
    {
        var userConnectionsKey = CacheKeys.UserConnections(userId);
        return await _cache.SetMembersAsync(userConnectionsKey);
    }

    public async Task<string[]> GetUserGroups(string userId)
    {
        var userGroupsKey = CacheKeys.UserGroups(userId);
        return await _cache.SetMembersAsync(userGroupsKey);
    }
}
