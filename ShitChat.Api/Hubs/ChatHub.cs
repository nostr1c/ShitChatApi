using Microsoft.AspNetCore.SignalR;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using ShitChat.Application.Groups.Services;
using Castle.Components.DictionaryAdapter;
using ShitChat.Application.Caching;

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

        // Add to SignalR group.
        await Groups.AddToGroupAsync(connectionId, groupId);

        // Update redis precence
        await _presenceService.AddConnectionToGroup(groupId, userId, connectionId);

        // Send new precense list to group.
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

        // Get users groups
        var groups = await _presenceService.GetUserGroups(userId);

        // Remove connection
        await _presenceService.RemoveConnection(userId, connectionId);

        // Send new precence for all users groups
        if (groups != null)
        {
            foreach (var groupId in groups)
            {
                var users = await _presenceService.GetUsersInGroup(groupId);
                await Clients.Group(groupId).SendAsync("PresenceUpdated", groupId, users);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
