using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Application.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Application.Interfaces;

namespace ShitChat.Api.Hubs;

public class ChatHub : Hub
{
    //private readonly ILogger<ChatHub> _logger;

    //public ChatHub(ILogger<ChatHub> logger)
    //{
    //    _logger = logger;
    //}

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task TypeIndicator(string roomId, string userId, bool isTyping)
    {
        await Clients.Group(roomId).SendAsync("ReceiveUserTyping", roomId, userId, isTyping);
    }

    public async Task ChangeAvatar(string userId, string imageName)
    {
        await Clients.All.SendAsync("ReceiveChangedAvatar", userId, imageName);
    }
}
