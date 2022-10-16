using MovieAPI.Data;

namespace MovieAPI.Models.DTO
{
    public class TicketDTO
    {
        public bool IsFromAdmin { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageTime { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}
