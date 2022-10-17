using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Services.SignalR;
using System.Reflection;

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
        private readonly ILogger<ChatController> logger;
        public ChatController(MovieAPIDbContext db, IHubContext<ChatHub> hub, IMapper mapper, ILogger<ChatController> logger)
        {
            context = db;
            this.hub = hub;
            this.mapper = mapper;
            this.logger = logger;
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetTopChat(int top)
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            try
            {
                var tickets = context.Tickets.Select(t => t.GroupID).Distinct().Take(top).ToList();
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
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var tickets = context.Tickets.Where(t => t.GroupID == GroupID).ToList();
                if (tickets == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Ticket", "Group not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Group not found"
                    });
                }
                var ticketsDTOs = mapper.Map<List<Ticket>, List<TicketDTO>>(tickets);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Ticket", tickets.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get chat message success",
                    Data = ticketsDTOs
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Ticket", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessageFromUser([FromBody] ChatModel chatModel)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
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
                    throw new Exception("Save data to of ticket database failed");
                }
                await hub.Clients.Group(chatModel.GroupID.ToString()).SendAsync("SendMessage", chatModel.Message);
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PostDataSuccess("Ticket"));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send message success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("Ticket", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SendMessageFromAdmin([FromBody] ChatModel chatModel)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
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
                    throw new Exception("Save data to of ticket database failed");
                }
                await hub.Clients.Group(chatModel.GroupID.ToString()).SendAsync("SendMessage", chatModel.Message);
                logger.LogInformation(MethodBase.GetCurrentMethod()!.Name.PostDataSuccess("Ticket"));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Send message success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PostDataError("Ticket", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
