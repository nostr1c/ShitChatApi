using Microsoft.AspNetCore.SignalR;

namespace ShitChat.Api.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string groupId, string userId, string content)
    {
        await Clients.Group(groupId).SendAsync("ReceiveMessage", new
        {
            GroupId = groupId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }
}
