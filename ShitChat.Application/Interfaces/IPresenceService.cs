namespace ShitChat.Application.Interfaces
{
    public interface IPresenceService
    {
        Task AddConnectionToGroup(string groupId, string userId, string connectionId);
        Task RemoveConnection(string userId, string connectionId);
        Task<string[]> GetUsersInGroup(string groupId);
        Task<string[]> GetUserConnections(string groupId, string userId);
        Task<string[]> GetConnectionGroups(string connectionId);
    }
}
