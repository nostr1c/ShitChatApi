using Microsoft.AspNetCore.SignalR;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using ShitChat.Application.Groups.Services;

namespace ShitChat.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IPresenceService _presenceService;

    public ChatHub(ILogger<ChatHub> logger, IPresenceService presenceService)
    {
        _logger = logger;
        _presenceService = presenceService;
    }

    public async Task JoinGroup(string groupId)
    {
        var userId = Context.User!.GetUserGuid();
        if (userId == null)
            return;

        var connectionId = Context.ConnectionId;

        await Groups.AddToGroupAsync(connectionId, groupId);
        await _presenceService.AddConnectionToGroup(groupId, userId, connectionId);

        var users = await _presenceService.GetUsersInGroup(groupId);
        await Clients.Group(groupId).SendAsync("PresenceUpdated", groupId, users);
    }

    public async Task TypeIndicator(string roomId, string userId, bool isTyping)
    {
        await Clients.Group(roomId).SendAsync("ReceiveUserTyping", roomId, userId, isTyping);
    }

    public async Task ChangeAvatar(string userId, string imageName)
    {
        await Clients.All.SendAsync("ReceiveChangedAvatar", userId, imageName);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User!.GetUserGuid();
        var connectionId = Context.ConnectionId;

        if (userId == null)
            return;

        var groupIds = await _presenceService.GetConnectionGroups(connectionId);

        await _presenceService.RemoveConnection(userId, connectionId);

        foreach (var groupId in groupIds)
        {
            var users = await _presenceService.GetUsersInGroup(groupId);
            await Clients.Group(groupId).SendAsync("PresenceUpdated", groupId, users);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
