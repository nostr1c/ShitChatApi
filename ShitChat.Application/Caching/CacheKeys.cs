namespace ShitChat.Application.Caching;

public static class CacheKeys
{
    public static string GroupMembers(Guid groupId) => $"shitchat:group:{groupId}:members";
    public static string GroupMessages(Guid groupId) => $"shitchat:group:{groupId}:recent-messages";
}
