using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MovieAPI.Data;
using MovieAPI.Data.DbConfig;
using MovieAPI.Helpers;
using MovieAPI.Models;
using MovieAPI.Models.DTO;
using MovieAPI.Services.Attributes;
using MovieAPI.Services.SignalR;
using System.Reflection;
using System.Text.RegularExpressions;

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
        public IActionResult GetAllUserChat()
        {
            logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
            try
            {
                var tickets = context.Tickets
                .Select(t => t.GroupID)
                .Distinct()
                .ToList();
                var Profiles = new List<object>();
                foreach (var ticket in tickets)
                {
                    var myTicket = context.Tickets
                        .Where(t=>t.GroupID==ticket)
                        .OrderByDescending(t=>t.MessageTime)
                        .Take(1).ToList();
                    if (myTicket[0].IsFromAdmin)
                    {
                        myTicket[0].IsRead = true;

                    }
                    var profile= context.Profiles.SingleOrDefault(pro => pro.UserID == ticket);
                    var ticketObject = new
                    {
                        UserID = profile.UserID,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        Message = myTicket[0].MessageContent,
                        Time = myTicket[0].MessageTime,
                        IsRead = myTicket[0].IsRead,
                        TicketID = myTicket[0].TicketID
                    };
                    Profiles.Add(ticketObject);
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.GetDataSuccess("Ticket", tickets.Count));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Get All Chat",
                    Data = Profiles
                });
            }
            catch(Exception ex)
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
        [HttpGet]
        public IActionResult GetChatMessage(Guid GroupID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var tickets = context.Tickets
                .Where(t => t.GroupID == GroupID)
                .OrderBy(t => t.MessageTime)
                .ToList();
                if (tickets.Count == 0)
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
        [UserBanned]
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
                    ReceiverId = chatModel.GroupID,
                    MessageContent = chatModel.Message,
                    MessageTime = DateTime.Now,
                    GroupID = chatModel.GroupID,
                    IsRead = false
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
                    GroupID = chatModel.GroupID,
                    IsRead = false
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
        [HttpPut]
        public IActionResult UpdateReadStatus(Guid TicketID)
        {
            try
            {
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.MethodStart());
                var ticket = context.Tickets.SingleOrDefault(t=>t.TicketID == TicketID);
                if (ticket == null)
                {
                    logger.LogError(MethodBase.GetCurrentMethod()!.Name.GetDataError("Ticket", "Ticket not found"));
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Ticket not failed"
                    });
                }
                ticket.IsRead = true;
                context.Tickets.Update(ticket);
                var returnValue = context.SaveChanges();
                if (returnValue == 0)
                {
                    throw new Exception("Update Data of ticket fealse");
                }
                logger.LogInformation(MethodBase.GetCurrentMethod().Name.PutDataSuccess("Ticket", 1));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Update ticket success"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(MethodBase.GetCurrentMethod()!.Name.PutDataError("Ticket", ex.ToString()));
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Internal Server Error"
                });
            }
        }
    }
}
