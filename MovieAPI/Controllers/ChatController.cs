using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;
using MovieAPI.Services.SignalR;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class ChatController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly IHubContext<ChatHub> hub;
        public ChatController(MovieAPIDbContext db, IHubContext<ChatHub> hub)
        {
            context = db;
            this.hub = hub;
        }
        //[Authorize]
        //[HttpPost]
        //public IActionResult GetAllTicket()
        //{

        //}
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessageFromUser([FromBody] ChatModel chatModel)
        {

            try
            {
                var ticket = new Ticket
                {
                    SenderId = chatModel.GroupID,
                    IsFromAdmin = false,
                    MessageContent = chatModel.Message,
                    MessageTime = DateTime.Now,
                    GroupID = chatModel.GroupID
                };
                context.Tickets.Add(ticket);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    return StatusCode(500, new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Something went wrong"
                    });
                }
                await hub.Clients.Group(chatModel.GroupID.ToString()).SendAsync("SendMessage", chatModel.Message);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send message success"
                });
            }
            catch 
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Something went wrong"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SendMessageFromAdmin([FromBody] ChatModel chatModel)
        {
            try
            {
                var ticket = new Ticket
                {
                    SenderId = chatModel.AdminID,
                    ReceiverId = chatModel.GroupID,
                    IsFromAdmin = true,
                    MessageContent = chatModel.Message,
                    MessageTime = DateTime.Now,
                    GroupID = chatModel.GroupID
                };
                context.Tickets.Add(ticket);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    return StatusCode(500, new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Something went wrong"
                    });
                }
                await hub.Clients.Group(chatModel.GroupID.ToString()).SendAsync("SendMessage", chatModel.Message);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send message success"
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Something went wrong"
                });
            }
        }
    }
}
