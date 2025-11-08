namespace ShitChat.Application.Caching;

public static class CacheKeys
{
    public static string GroupMembers(Guid groupId) => $"shitchat:group:{groupId}:members";
    public static string GroupMessages(Guid groupId) => $"shitchat:group:{groupId}:recent-messages";
    public static string GroupUsers(string groupId) => $"Presence:Group:${groupId}";
    public static string UserGroups(string userId) => $"Presence:UserGroups:${userId}";
    public static string UserConnections(string userId) => $"Presence:UserConnections:${userId}";
}
