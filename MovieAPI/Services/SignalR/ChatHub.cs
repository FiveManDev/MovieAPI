using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using MovieAPI.Controllers;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;

namespace MovieAPI.Services.SignalR
{
    public class ChatHub : Hub
    {
        public async Task JoinGroup(Guid GroupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName.ToString());
        }
        public async Task RemoveFromGroup(Guid GroupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName.ToString());
        }

    }
}
