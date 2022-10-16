using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Services.SignalR;
using Serilog.Context;

namespace MovieAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[Action]")]
    [ApiController]
    [ApiVersion("1")]
    public class ChatController : ControllerBase
    {
        private readonly MovieAPIDbContext context;
        private readonly IHubContext<ChatHub> hub;
        private readonly IMapper mapper;
        public ChatController(MovieAPIDbContext db, IHubContext<ChatHub> hub, IMapper mapper)
        {
            context = db;
            this.hub = hub;
            this.mapper = mapper;
        }
        [Authorize]
        [HttpPost]
        public IActionResult GetTopChat(int top)
        {
            try
            {
                var tickets = context.Tickets.Select(t=>t.GroupID).Distinct().Take(top).ToList();
                return Ok(tickets);
            }
            catch
            {
                return Ok(top);
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetChatMessage(Guid GroupID)
        {
            try
            {
                var tikets = context.Tickets.Where(t => t.GroupID == GroupID).ToList();
                if (tikets == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Message="Get chat from group id faild"
                    });
                }
                List<TicketDTO> tiketsDTOs = new List<TicketDTO>();
                foreach(var ticket in tikets)
                {
                    tiketsDTOs.Add(mapper.Map<Ticket,TicketDTO>(ticket));
                }
                return Ok(new ApiResponse
                {
                    IsSuccess=true,
                    Message= "Get chat message success",
                    Data= tiketsDTOs
                });
            }
            catch
            {
                return StatusCode(500,new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Get Chat Message failed"
                });
            }
        }
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
