namespace MovieAPI.Data
{
    public class Ticket
    {
        public Guid TicketID { get; set; }
        public Guid GroupID { get; set; }
        public bool IsFromAdmin { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageTime { get; set; }
        public bool IsRead { get; set; }
        //Relationship
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
