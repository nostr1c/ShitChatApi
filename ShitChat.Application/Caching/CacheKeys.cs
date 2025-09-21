namespace ShitChat.Application.Caching;

public static class CacheKeys
{
    public static string GroupMembers(Guid groupId) => $"shitchat:group:{groupId}:members";
    public static string GroupMessages(Guid groupId) => $"shitchat:group:{groupId}:recent-messages";
    public static string GroupUsers(string groupId) => $"presence:group:{groupId}";
    public static string UserConnections(string groupId, string userId) => $"presence:connections:{groupId}:{userId}";
    public static string ConnectionGroups(string connectionId) => $"presence:connections:{connectionId}";

}
