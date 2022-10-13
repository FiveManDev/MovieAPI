using Microsoft.AspNetCore.SignalR;

namespace MovieAPI.Services.SignalR
{
    public class ReviewHub:Hub
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
