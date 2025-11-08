namespace ShitChat.Application.Groups.Services
{
    public interface IPresenceService
    {
        Task AddConnectionToGroup(string groupId, string userId, string connectionId);
        Task RemoveConnection(string userId, string connectionId);
        Task<string[]> GetUsersInGroup(string groupId);
        Task<string[]> GetUserConnections(string userId);
        Task<string[]> GetUserGroups(string userId);
    }
}
