using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ShitChat.Application.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Application.Interfaces;

namespace ShitChat.Api.Hubs;

public class ChatHub : Hub
{
    //public async Task SendMessage(string groupId, string userId, string content)
    //{
    //    await Clients.Group(groupId).SendAsync("ReceiveMessage", new MessageDto
    //    {
    //        Id

    //        GroupId = groupId,
    //        UserId = userId,
    //        Content = content,
    //        CreatedAt = DateTime.UtcNow
    //    });
    //}

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }
}
